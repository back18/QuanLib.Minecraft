using FFMediaToolkit.Decoding;
using FFMediaToolkit.Graphics;
using Microsoft.VisualBasic;
using NAudio.Midi;
using NAudio.Wave;
using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.BlockScreen.Frame;
using SixLabors.ImageSharp;
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
        public VideoBox()
        {
            DefaultMediaOptions = new();
            DefaultResizeOptions = VideoFrame.DefaultResizeOptions.Clone();
            DefaultResizeOptions.Size = ClientSize;
            ClientSize = new(64, 64);
            ContentAnchor = AnchorPosition.Centered;

            Played += OnPlayed;
            Paused += OnPaused;
            VideoFrameChanged += OnVideoFrameChanged;
            MediaFilePlayerChanged += OnMediaFilePlayerChanged;
        }

        public MediaOptions DefaultMediaOptions { get; }

        public ResizeOptions DefaultResizeOptions { get; }

        public MediaFilePlayer? MediaFilePlayer
        {
            get => _MediaFilePlayer;
            set
            {
                MediaFilePlayer? temp = _MediaFilePlayer;
                _MediaFilePlayer = value;
                MediaFilePlayerChanged.Invoke(this, new(temp, _MediaFilePlayer));
                RequestUpdateFrame();
            }
        }
        private MediaFilePlayer? _MediaFilePlayer;

        public TimeSpan CurrentPosition => MediaFilePlayer?.CurrentPosition ?? TimeSpan.Zero;

        public TimeSpan TotalTime => MediaFilePlayer?.TotalTime ?? TimeSpan.Zero;

        public event EventHandler<VideoBox, EventArgs> Played;

        public event EventHandler<VideoBox, EventArgs> Paused;

        public event EventHandler<VideoBox, VideoFrameChangedEventArgs> VideoFrameChanged;

        public event EventHandler<VideoBox, MediaFilePlayerChangedEventArge> MediaFilePlayerChanged;

        protected virtual void OnPlayed(VideoBox sender, EventArgs e) { }

        protected virtual void OnPaused(VideoBox sender, EventArgs e) { }

        protected virtual void OnVideoFrameChanged(VideoBox sender, VideoFrameChangedEventArgs e)
        {
            RequestUpdateFrame();
        }

        protected virtual void OnMediaFilePlayerChanged(VideoBox sender, MediaFilePlayerChangedEventArge e)
        {
            e.OldMediaFilePlayer?.Dispose();

            if (e.OldMediaFilePlayer is not null)
            {
                e.OldMediaFilePlayer.Played -= NewMediaFilePlayer_Played;
                e.OldMediaFilePlayer.Paused -= NewMediaFilePlayer_Paused;
                e.OldMediaFilePlayer.VideoFrameChanged -= NewMediaFilePlayer_VideoFrameChanged;
            }

            if (e.NewMediaFilePlayer is not null)
            {
                e.NewMediaFilePlayer.Played += NewMediaFilePlayer_Played;
                e.NewMediaFilePlayer.Paused += NewMediaFilePlayer_Paused;
                e.NewMediaFilePlayer.VideoFrameChanged += NewMediaFilePlayer_VideoFrameChanged;
            }
        }

        protected override void OnResize(Control sender, SizeChangedEventArgs e)
        {
            base.OnResize(sender, e);

            Size offset = e.NewSize - e.OldSize;
            DefaultResizeOptions.Size += offset;
            if (MediaFilePlayer is not null)
                MediaFilePlayer.VideoDecoder.ResizeOptions.Size += offset;
        }

        protected override void OnAfterFrame(Control sender, EventArgs e)
        {
            base.OnAfterFrame(sender, e);

            MediaFilePlayer?.Handle();
        }

        public override IFrame RenderingFrame()
        {
            if (MediaFilePlayer is null || MediaFilePlayer.CurrentVideoFrame is null)
            {
                return base.RenderingFrame();
            }

            if (MediaFilePlayer.CurrentVideoFrame.FrameSize != ClientSize)
            {
                MediaFilePlayer.CurrentVideoFrame.ResizeOptions.Size = ClientSize;
                MediaFilePlayer.CurrentVideoFrame.Update();
            }

            return MediaFilePlayer.CurrentVideoFrame.GetFrameClone();
        }

        private void NewMediaFilePlayer_Played(MediaFilePlayer sender, EventArgs e)
        {
            Played.Invoke(this, e);
        }

        private void NewMediaFilePlayer_Paused(MediaFilePlayer sender, EventArgs e)
        {
            Paused.Invoke(this, e);
        }

        private void NewMediaFilePlayer_VideoFrameChanged(MediaFilePlayer sender, VideoFrameChangedEventArgs e)
        {
            VideoFrameChanged.Invoke(this, e);
        }

        public bool TryReadMediaFile(string path)
        {
            if (!File.Exists(path))
                return false;

            try
            {
                MediaFilePlayer = new(path, GetScreenPlane().NormalFacing, DefaultMediaOptions, DefaultResizeOptions);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            MediaFilePlayer?.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
