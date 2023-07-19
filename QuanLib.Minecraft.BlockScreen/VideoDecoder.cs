using FFMediaToolkit.Decoding;
using FFMediaToolkit.Graphics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public class VideoDecoder : ISwitchable
    {
        public VideoDecoder(VideoStream video)
        {
            _video = video ?? throw new ArgumentNullException(nameof(video));

            CacheFrames = 16;
            FrameInterval = 0;
            FrameInterval = Math.Round(1000 / video.Info.AvgFrameRate);

            _frames = new();
            _semaphore = new(1);
            _pause = false;
            _runing = false;
        }

        private readonly ConcurrentQueue<Task<VideoFrame?>> _frames;

        public VideoStream _video;

        private readonly SemaphoreSlim _semaphore;

        private bool _pause;

        private bool _runing;

        public bool Runing => _runing;

        public int CacheFrames { get; set; }

        public TimeSpan DecodingEndPosition
        {
            get => _video.Position;
            set => TryJumpToFrame(value);
        }

        public TimeSpan CurrentPosition
        {
            get => _CurrentTime;
            set => TryJumpToFrame(value);
        }
        private TimeSpan _CurrentTime;

        public double FrameInterval { get; }

        public void Start()
        {
            _runing = true;
            while (_runing)
            {
                if (_frames.Count >= CacheFrames)
                    Thread.Sleep(10);
                else
                {
                    while (_pause)
                        Thread.Yield();
                    if (!_runing)
                        break;

                    _frames.Enqueue(Task.Run(() =>
                    {
                        _semaphore.Wait();
                        ImageData imageData;
                        try
                        {
                            imageData = _video.GetNextFrame();
                        }
                        catch
                        {
                            return null;
                        }
                        finally
                        {
                            _semaphore.Release();
                        }
                        return VideoFrame.FromImageData(_video.Position, imageData.Data, imageData.ImageSize.Width, imageData.ImageSize.Height);
                    }));
                }
            }
        }

        public void Stop()
        {
            _pause = true;
            _runing = false;
            Clear();
            _pause = false;
        }

        public void Clear()
        {
            while (!_frames.IsEmpty)
            {
                if (_frames.TryDequeue(out var frame))
                    frame.ContinueWith(t => t.Result?.Image.Dispose());
            }
        }

        public bool TryGetNextFrame([MaybeNullWhen(false)] out VideoFrame result)
        {
            while (true)
            {
                if (!_runing)
                {
                    result = null;
                    return false;
                }
                else if (_frames.IsEmpty)
                {
                    Thread.Yield();
                }
                else
                {
                    if (_frames.TryDequeue(out var task))
                    {
                        if (task.Result is null)
                        {
                            result = null;
                            return false;
                        }
                        else
                        {
                            _CurrentTime = task.Result.Position;
                            result = task.Result;
                            return true;
                        }
                    }
                }
            }
        }

        public bool TryJumpToFrame(TimeSpan time)
        {
            _pause = true;
            Clear();

            _semaphore.Wait();
            ImageData imageData;
            try
            {
                if (_video is null || !_video.TryGetFrame(time, out imageData))
                    return false;
            }
            finally
            {
                _semaphore.Release();
            }
            VideoFrame frame = VideoFrame.FromImageData(_video.Position, imageData.Data, imageData.ImageSize.Width, imageData.ImageSize.Height);

            _frames.Enqueue(Task.Run<VideoFrame?>(() => frame));
            _pause = false;
            return true;
        }
    }
}
