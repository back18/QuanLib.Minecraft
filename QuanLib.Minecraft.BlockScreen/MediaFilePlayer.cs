using FFMediaToolkit.Decoding;
using NAudio.Wave;
using Newtonsoft.Json.Linq;
using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.BlockScreen.Frame;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public class MediaFilePlayer : IDisposable
    {
        public MediaFilePlayer(string path, Facing facing, MediaOptions mediaOptions, ResizeOptions resizeOptions, bool enableAudio = true)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();

            MediaFile = MediaFile.Open(path, mediaOptions);
            VideoDecoder = new(MediaFile.Video, facing, resizeOptions);
            EnableAudio = enableAudio;
            if (EnableAudio)
            {
                MediaFoundationReader = new(path);
                WaveOutEvent = new();
                WaveOutEvent.Init(MediaFoundationReader);
            }

            _start = TimeSpan.Zero;
            _stopwatch = new();
            PlayerState = MediaFilePlayerState.Unstarted;

            VideoFrameChanged += OnVideoFrameChanged;
            Played += OnPlayed;
            Paused += OnPaused;
        }

        private TimeSpan _start;

        private readonly Stopwatch _stopwatch;

        public bool EnableAudio { get; set; }

        public MediaFile MediaFile { get; }

        public VideoDecoder VideoDecoder { get; }

        public MediaFoundationReader? MediaFoundationReader { get; }

        public WaveOutEvent? WaveOutEvent { get; }

        public VideoFrame? CurrentVideoFrame { get; private set; }

        public TimeSpan CurrentPosition
        {
            get => VideoDecoder.CurrentPosition;
            set => TryJumpToFrame(value);
        }

        public TimeSpan TotalTime => MediaFile.Info.Duration;

        public MediaFilePlayerState PlayerState { get; private set; }

        public event EventHandler<MediaFilePlayer, EventArgs> Played;

        public event EventHandler<MediaFilePlayer, EventArgs> Paused;

        public event EventHandler<MediaFilePlayer, VideoFrameChangedEventArgs> VideoFrameChanged;

        protected virtual void OnPlayed(MediaFilePlayer sender, EventArgs e) { }

        protected virtual void OnPaused(MediaFilePlayer sender, EventArgs e) { }

        protected virtual void OnVideoFrameChanged(MediaFilePlayer sender, VideoFrameChangedEventArgs e)
        {
            e.OldVideoFrame?.Dispose();
        }

        public bool TryJumpToFrame(TimeSpan time)
        {
            if (VideoDecoder.TryJumpToFrame(time))
            {
                _start = time;
                if (MediaFoundationReader is not null)
                    MediaFoundationReader.CurrentTime = _start;
                _stopwatch.Restart();
                _stopwatch.Start();
                Play();
                return true;
            }
            return false;
        }

        public void Play()
        {
            if (PlayerState != MediaFilePlayerState.Playing)
            {
                lock (VideoDecoder)
                {
                    if (!VideoDecoder.Runing)
                        Task.Run(() => VideoDecoder.Start());
                }

                WaveOutEvent?.Play();
                _stopwatch.Start();

                PlayerState = MediaFilePlayerState.Playing;
                Played.Invoke(this, EventArgs.Empty);
            }
            else
            {
                PlayerState = MediaFilePlayerState.Playing;
            }
        }

        public void Pause()
        {
            if (PlayerState != MediaFilePlayerState.Pause)
            {
                WaveOutEvent?.Stop();
                _stopwatch.Stop();

                PlayerState = MediaFilePlayerState.Pause;
                Paused.Invoke(this, EventArgs.Empty);
            }
            else
            {
                PlayerState = MediaFilePlayerState.Pause;
            }
        }

        public void Dispose()
        {
            if (PlayerState != MediaFilePlayerState.Ended)
            {
                Pause();

                VideoDecoder.Stop();
                MediaFile.Dispose();
                WaveOutEvent?.Dispose();
                MediaFoundationReader?.Dispose();
                CurrentVideoFrame?.Dispose();

                GC.SuppressFinalize(this);

                PlayerState = MediaFilePlayerState.Ended;
            }
        }

        public void Handle()
        {
            if (PlayerState != MediaFilePlayerState.Playing)
                return;

            VideoFrame? frame;
            while (true)
            {
                if (!VideoDecoder.TryGetNextFrame(out frame))
                {
                    Pause();
                    break;
                }

                double next = frame.Position.TotalMilliseconds + VideoDecoder.FrameInterval;
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

            VideoFrame? temp = CurrentVideoFrame;
            CurrentVideoFrame = frame;
            VideoFrameChanged.Invoke(this, new(temp, CurrentVideoFrame));
        }
    }
}
