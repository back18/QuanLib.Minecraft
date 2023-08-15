using FFMediaToolkit.Decoding;
using QuanLib.Minecraft.Block;
using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.BlockScreen.Frame;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public class VideoProgressBar : HorizontalProgressBar
    {
        public VideoProgressBar(VideoPlayerBox owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));

            Time_Label = new();

            Skin.SetAllForegroundBlockID(BlockManager.Concrete.Pink);

            _owner.VideoBox.VideoFrameChanged += VideoPlayer_VideoFrameChanged;
        }

        protected readonly VideoPlayerBox _owner;

        private readonly Label Time_Label;

        public override void Initialize()
        {
            base.Initialize();

            if (_owner != ParentContainer)
                throw new InvalidOperationException();
        }

        private void VideoPlayer_VideoFrameChanged(VideoBox sender, VideoFrameChangedEventArgs e)
        {
            RequestUpdateFrame();
            Progress = _owner.VideoBox.CurrentPosition / _owner.VideoBox.TotalTime;
        }

        protected override void OnCursorMove(Control sender, CursorEventArgs e)
        {
            base.OnCursorMove(sender, e);

            if (_owner.SubControls.Contains(Time_Label))
            {
                Time_Label.Text = VideoPlayerBox.FromTimeSpan(GetProgressBarPosition(e.Position.X));
                Time_Label.AutoSetSize();
                int x = e.Position.X - Time_Label.Width / 2;
                Time_Label.ClientLocation = new(x, TopLocation - Time_Label.Height - 2);
            }
        }

        protected override void OnCursorEnter(Control sender, CursorEventArgs e)
        {
            base.OnCursorEnter(sender, e);

            _owner.SubControls.TryAdd(Time_Label);
        }

        protected override void OnCursorLeave(Control sender, CursorEventArgs e)
        {
            base.OnCursorLeave(sender, e);

            _owner.SubControls.Remove(Time_Label);
        }

        protected override void OnRightClick(Control sender, CursorEventArgs e)
        {
            base.OnRightClick(sender, e);

            if (_owner.VideoBox.MediaFilePlayer is null)
                return;

            _owner.VideoBox.MediaFilePlayer.CurrentPosition = GetProgressBarPosition(e.Position.X);
        }

        private TimeSpan GetProgressBarPosition(int x)
        {
            return _owner.VideoBox.TotalTime * ((double)x / ClientSize.Width);
        }
    }
}
