using FFMediaToolkit.Decoding;
using FFMediaToolkit.Graphics;
using Microsoft.VisualBasic;
using NAudio.Wave;
using QuanLib.Minecraft.BlockScreen.Frame;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public class VideoBox : Control, IDisposable
    {
        static VideoBox()
        {
            DefaultMediaOptions = new();
            DefaultResizeOptions = new()
            {
                Size = new(64, 36),
                Mode = ResizeMode.Pad,
                Position = AnchorPositionMode.Center,
                Sampler = KnownResamplers.Bicubic
            };
        }

        public VideoBox()
        {
            PlayerState = VideoPlayerState.Unstarted;
            MediaOptions = DefaultMediaOptions.Copy();
            ResizeOptions = DefaultResizeOptions.Copy();

            AutoSize = true;
            ContentAnchor = AnchorPosition.Centered;

            OnStartedPlay += VideoPlayer_OnStartedPlay;
            OnEndedPlay += VideoPlayer_OnEndedPlay;
            OnPause += VideoPlayer_OnPause;
            OnResume += VideoPlayer_OnResume;
            OnVideoFrameUpdate += (obj) => { };
            OnMediaFileUpdate += VideoPlayer_OnMediaFileUpdate;

            _decoder = null;
            _audio = null;
            _event = null;
            _start = TimeSpan.Zero;
            _stopwatch = new();

            AfterFrame += VideoPlayer_AfterFrame;
        }

        public static MediaOptions DefaultMediaOptions { get; }

        public static ResizeOptions DefaultResizeOptions { get; }

        private VideoDecoder? _decoder;

        private MediaFoundationReader? _audio;

        private WaveOutEvent? _event;

        private TimeSpan _start;

        private readonly Stopwatch _stopwatch;

        public VideoPlayerState PlayerState
        {
            get => _PlayerState;
            private set
            {
                if (_PlayerState != value)
                {
                    VideoPlayerState temp = _PlayerState;
                    _PlayerState = value;

                    if ((temp == VideoPlayerState.Unstarted || temp == VideoPlayerState.Ended) && _PlayerState == VideoPlayerState.Playing)
                    {
                        OnStartedPlay.Invoke();
                    }

                    switch (_PlayerState)
                    {
                        case VideoPlayerState.Playing:
                            OnResume.Invoke();
                            break;
                        case VideoPlayerState.Pause:
                            OnPause.Invoke();
                            break;
                        case VideoPlayerState.Ended:
                            OnEndedPlay.Invoke();
                            break;
                    }
                }
            }
        }
        private VideoPlayerState _PlayerState;

        public TimeSpan CurrentPosition
        {
            get => _decoder?.CurrentPosition ?? TimeSpan.Zero;
            set
            {
                _stopwatch.Restart();
                _stopwatch.Start();
                if (PlayerState != VideoPlayerState.Playing)
                    Play();
                _decoder?.TryJumpToFrame(value);
                if (_audio is not null)
                    _audio.CurrentTime = value;
                _start = value;
            }
        }

        public TimeSpan TotalTime => MediaFile?.Info.Duration ?? TimeSpan.Zero;

        public MediaFile? MediaFile
        {
            get => _MediaFile;
            private set
            {
                _MediaFile?.Dispose();
                _MediaFile = value;
                if (AutoSize)
                    AutoSetSize();
                OnMediaFileUpdate.Invoke(_MediaFile);
                RequestUpdateFrame();
            }
        }
        private MediaFile? _MediaFile;

        public VideoFrame? CurrentVideoFrame { get; private set; }

        public MediaOptions MediaOptions { get; }

        public ResizeOptions ResizeOptions { get; }

        public event Action OnStartedPlay;

        public event Action OnEndedPlay;

        public event Action OnPause;

        public event Action OnResume;

        public event Action<VideoFrame> OnVideoFrameUpdate;

        public event Action<MediaFile?> OnMediaFileUpdate;

        public override IFrame RenderingFrame()
        {
            if (CurrentVideoFrame is not null)
            {
                CurrentVideoFrame.Image.Mutate(x => x.Resize(ResizeOptions));
                return ArrayFrame.FromImage(GetMCOS().Screen.NormalFacing, CurrentVideoFrame.Image);
            }
            else
            {
                return ArrayFrame.BuildFrame(ResizeOptions.Size.Width, ResizeOptions.Size.Height, Skin.GetBackgroundBlockID());
            }
        }

        public override void AutoSetSize()
        {
            ClientSize = ResizeOptions.Size;
        }

        private void VideoPlayer_OnStartedPlay()
        {
            if (_decoder is not null)
            {
                _stopwatch.Restart();
                _start = TimeSpan.Zero;
                if (_decoder.Runing)
                    _decoder.TryJumpToFrame(_start);
                else
                {
                    if (_decoder.CurrentPosition != _start)
                        _decoder.TryJumpToFrame(_start);
                    Task.Run(() => _decoder.Start());
                }
            }
        }

        private void VideoPlayer_OnEndedPlay()
        {
            _stopwatch.Stop();
            _decoder?.Stop();
            _event?.Stop();
        }

        private void VideoPlayer_OnPause()
        {
            _stopwatch.Stop();
            _event?.Pause();
        }

        private void VideoPlayer_OnResume()
        {
            _stopwatch.Start();
            _event?.Play();
        }

        private void VideoPlayer_OnMediaFileUpdate(MediaFile? mediaFile)
        {
            _stopwatch.Stop();
            _decoder?.Stop();
            if (mediaFile is null)
                _decoder = null;
            else
                _decoder = new(mediaFile.Video);
            PlayerState = VideoPlayerState.Unstarted;
        }

        private void VideoPlayer_AfterFrame()
        {
            if (_decoder is not null && PlayerState == VideoPlayerState.Playing)
            {
                NextFrame();
                RequestUpdateFrame();
            }
        }

        public void Play()
        {
            if (_decoder is not null)
            {
                PlayerState = VideoPlayerState.Playing;
            }
        }

        public void StartPlay()
        {
            PlayerState = VideoPlayerState.Playing;
        }

        public void EndPlay()
        {
            PlayerState = VideoPlayerState.Ended;
        }

        public void Pause()
        {
            PlayerState = VideoPlayerState.Pause;
        }

        public void Resume()
        {
            PlayerState = VideoPlayerState.Playing;
        }

        private void NextFrame()
        {
            if (!AllowGetApplication())
                return;

            if (_decoder is not null)
            {
                VideoFrame? frame;
                while (true)
                {
                    if (!_decoder.TryGetNextFrame(out frame))
                    {
                        EndPlay();
                        return;
                    }

                    double next = frame.Position.TotalMilliseconds + _decoder.FrameInterval;
                    double now = (_start + _stopwatch.Elapsed).TotalMilliseconds;
                    if (Math.Abs(now - frame.Position.TotalMilliseconds) < Math.Abs(now - next))
                    {
                        break;
                    }
                    else
                    {
                        frame.Image.Dispose();
                    }
                }

                CurrentVideoFrame = frame;
                OnVideoFrameUpdate.Invoke(CurrentVideoFrame);
            }
        }

        public bool TryReadMediaFile(string path)
        {
            try
            {
                MediaFile = MediaFile.Open(path, MediaOptions);
                try
                {
                    _event?.Dispose();
                    _audio?.Dispose();
                    _audio = new(path);
                }
                catch
                {
                    _audio = null;
                }
                if (_audio is not null)
                {
                    _event = new();
                    _event.Init(_audio);
                }
                else
                {
                    _event = null;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            _decoder?.Stop();
            MediaFile?.Dispose();
            _event?.Dispose();
            _audio?.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
