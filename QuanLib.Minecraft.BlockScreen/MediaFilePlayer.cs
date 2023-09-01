using FFMediaToolkit.Decoding;
using log4net.Core;
using NAudio.Wave;
using Newtonsoft.Json.Linq;
using QuanLib.Core;
using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.BlockScreen.Frame;
using QuanLib.Minecraft.BlockScreen.Logging;
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
        private static readonly LogImpl LOGGER = LogUtil.MainLogger;

        public MediaFilePlayer(string path, Facing facing, MediaOptions mediaOptions, ResizeOptions resizeOptions, bool enableAudio = true)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();

            MediaFile = MediaFile.Open(path, mediaOptions);
            VideoDecoder = new(MediaFile.Video, facing, resizeOptions);
            EnableAudio = enableAudio;
            if (EnableAudio)
            {
                try
                {
                    MediaFoundationReader = new(path);
                    WaveOutEvent = new();
                    WaveOutEvent.Init(MediaFoundationReader);
                    WaveOutEvent.Volume = 0.25f;
                }
                catch (Exception ex)
                {
                    WaveOutEvent?.Dispose();
                    MediaFoundationReader?.Dispose();
                    WaveOutEvent = null;
                    MediaFoundationReader = null;
                    EnableAudio = false;
                    LOGGER.Error("无法使用NAudio库播放音频，音频已禁用", ex);
                }
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

        public bool EnableAudio { get; }

        public MediaFile MediaFile { get; }

        public VideoDecoder VideoDecoder { get; }

        public MediaFoundationReader? MediaFoundationReader { get; }

        public WaveOutEvent? WaveOutEvent { get; }

        public VideoFrame? CurrentVideoFrame { get; private set; }

        public float Volume
        {
            get => WaveOutEvent?.Volume ?? 0;
            set
            {
                if (WaveOutEvent is null)
                    return;

                if (value < 0)
                    value = 0;
                else if (value > 1)
                    value = 1;

                WaveOutEvent.Volume = value;
            }
        }

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

                if (EnableAudio)
                {
                    if (MediaFoundationReader is not null)
                        MediaFoundationReader.CurrentTime = _start;
                    WaveOutEvent?.Play();
                }

                _stopwatch.Restart();
                _stopwatch.Start();
                if (PlayerState != MediaFilePlayerState.Playing)
                {
                    PlayerState = MediaFilePlayerState.Playing;
                    Played.Invoke(this, EventArgs.Empty);
                }
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

                if (EnableAudio)
                {
                    WaveOutEvent?.Play();
                }

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
                if (EnableAudio)
                {
                    WaveOutEvent?.Stop();
                }

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
