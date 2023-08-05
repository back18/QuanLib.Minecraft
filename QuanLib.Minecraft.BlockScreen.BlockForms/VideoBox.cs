using FFMediaToolkit.Decoding;
using FFMediaToolkit.Graphics;
using Microsoft.VisualBasic;
using NAudio.Midi;
using NAudio.Wave;
using QuanLib.Minecraft.BlockScreen.Event;
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
            ResizeOptions = DefaultResizeOptions.Clone();

            AutoSize = true;
            ContentAnchor = AnchorPosition.Centered;

            StartedPlay += OnStartedPlay;
            EndedPlay += OnEndedPlay;
            Pause += OnPause;
            Resume += OnResume;
            VideoFrameChanged += OnVideoFrameChanged;
            MediaFileChanged += OnMediaFileChanged;

            _decoder = null;
            _audio = null;
            _event = null;
            _start = TimeSpan.Zero;
            _stopwatch = new();
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
                        StartedPlay.Invoke(this, EventArgs.Empty);
                    }

                    switch (_PlayerState)
                    {
                        case VideoPlayerState.Playing:
                            Resume.Invoke(this, EventArgs.Empty);
                            break;
                        case VideoPlayerState.Pause:
                            Pause.Invoke(this, EventArgs.Empty);
                            break;
                        case VideoPlayerState.Ended:
                            EndedPlay.Invoke(this, EventArgs.Empty);
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
                MediaFileChanged.Invoke(this, new(_MediaFile));
                RequestUpdateFrame();
            }
        }
        private MediaFile? _MediaFile;

        public VideoFrame? CurrentVideoFrame { get; private set; }

        public MediaOptions MediaOptions { get; }

        public ResizeOptions ResizeOptions { get; }

        public event EventHandler<VideoBox, EventArgs> StartedPlay;

        public event EventHandler<VideoBox, EventArgs> EndedPlay;

        public event EventHandler<VideoBox, EventArgs> Pause;

        public event EventHandler<VideoBox, EventArgs> Resume;

        public event EventHandler<VideoBox, VideoFrameChangedEventArgs> VideoFrameChanged;

        public event EventHandler<VideoBox, MediaFileChangedEventArge> MediaFileChanged;

        protected override void OnAfterFrame(Control sender, EventArgs e)
        {
            base.OnAfterFrame(sender, e);

            if (_decoder is not null && PlayerState == VideoPlayerState.Playing)
            {
                NextFrame();
                RequestUpdateFrame();
            }
        }

        protected virtual void OnStartedPlay(VideoBox sender, EventArgs e)
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

        protected virtual void OnEndedPlay(VideoBox sender, EventArgs e)
        {
            _stopwatch.Stop();
            _decoder?.Stop();
            _event?.Stop();
        }

        protected virtual void OnPause(VideoBox sender, EventArgs e)
        {
            _stopwatch.Stop();
            _event?.Pause();
        }

        protected virtual void OnResume(VideoBox sender, EventArgs e)
        {
            _stopwatch.Start();
            _event?.Play();
        }

        protected virtual void OnVideoFrameChanged(VideoBox sender, VideoFrameChangedEventArgs e)
        {

        }

        protected virtual void OnMediaFileChanged(VideoBox sender, MediaFileChangedEventArge e)
        {
            _stopwatch.Stop();
            _decoder?.Stop();
            if (e.MediaFile is null)
                _decoder = null;
            else
                _decoder = new(e.MediaFile.Video);
            PlayerState = VideoPlayerState.Unstarted;
        }

        public override void AutoSetSize()
        {
            ClientSize = ResizeOptions.Size;
        }

        public override IFrame RenderingFrame()
        {
            if (CurrentVideoFrame is not null)
            {
                CurrentVideoFrame.Image.Mutate(x => x.Resize(ResizeOptions));
                return ArrayFrame.FromImage(GetScreenPlaneSize().NormalFacing, CurrentVideoFrame.Image);
            }
            else
            {
                return ArrayFrame.BuildFrame(ResizeOptions.Size.Width, ResizeOptions.Size.Height, Skin.GetBackgroundBlockID());
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

        public void Pauseing()
        {
            PlayerState = VideoPlayerState.Pause;
        }

        public void Resumeing()
        {
            PlayerState = VideoPlayerState.Playing;
        }

        private void NextFrame()
        {
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
                VideoFrameChanged.Invoke(this, new(CurrentVideoFrame));
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
