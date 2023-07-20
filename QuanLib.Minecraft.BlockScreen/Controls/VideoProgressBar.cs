using FFMediaToolkit.Decoding;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Controls
{
    public class VideoProgressBar : VideoPlayerSubControl
    {
        public VideoProgressBar(VideoPlayer owner) : base(owner)
        {
            Time_Label = new();

            Skin.SetAllForegroundBlockID(ConcretePixel.ToBlockID(MinecraftColor.LightBlue));

            CursorEnter += VideoProgressBar_CursorEnter;
            CursorLeave += VideoProgressBar_CursorLeave;
            CursorMove += VideoProgressBar_CursorMove;
            RightClick += VideoProgressBar_RightClick;

            _owner.OnVideoFrameUpdate += VideoPlayer_OnVideoFrameUpdate;
        }

        private readonly Label Time_Label;

        public override void Initialize()
        {
            base.Initialize();

            if (_owner != ParentContainer)
                throw new InvalidOperationException();
        }

        private void VideoPlayer_OnVideoFrameUpdate(VideoFrame frame)
        {
            RequestUpdateFrame();
        }

        private void VideoProgressBar_CursorEnter(Point position, CursorMode mode)
        {
            _owner.SubControls.TryAdd(Time_Label);
        }

        private void VideoProgressBar_CursorLeave(Point position, CursorMode mode)
        {
            _owner.SubControls.Remove(Time_Label);
        }

        private void VideoProgressBar_CursorMove(Point position, CursorMode mode)
        {
            if (_owner.SubControls.Contains(Time_Label))
            {
                Time_Label.Text = VideoPlayer.FromTimeSpan(GetProgressBarPosition(position.X));
                Time_Label.AutoSetSize();
                int x = position.X - Time_Label.Width / 2;
                //if (x < 0)
                //    x = 0;
                Time_Label.ClientLocation = new(x, TopLocation - Time_Label.Height - 2);
            }
        }

        private void VideoProgressBar_RightClick(Point position)
        {
            _owner.CurrentPosition = GetProgressBarPosition(position.X);
        }

        public override Frame RenderingFrame()
        {
            double proportion = _owner.CurrentPosition / _owner.TotalTime;
            int length = (int)(ClientSize.Width * proportion);

            FrameBuilder fb = new();
            if (length > 0)
            {
                fb.AddRight(Skin.GetForegroundBlockID(), length);
                fb.AddBottom(Skin.GetForegroundBlockID(), ClientSize.Height);
            }
            fb.AddRight(Skin.GetBackgroundBlockID(), ClientSize.Width - length);

            CorrectSize(fb);

            return fb.ToFrame();
        }

        private TimeSpan GetProgressBarPosition(int x)
        {
            return _owner.TotalTime * ((double)x / ClientSize.Width);
        }
    }
}
