using FFMediaToolkit.Decoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Controls
{
    public class VideoTimeTextBox : VideoPlayerSubControl
    {
        public VideoTimeTextBox(VideoPlayer owner) : base(owner)
        {
            CurrentTime_TextBox = new();
            TotalTime_Label = new();

            BorderWidth = 0;
            Skin.SetAllBackgroundBlockID(string.Empty);

            _owner.OnVideoFrameUpdate += VideoPlayer_OnVideoFrameUpdate;
            _owner.OnMediaFileUpdate += VideoPlayer__OnMediaFileUpdate;
        }

        private readonly TextBox CurrentTime_TextBox;

        private readonly Label TotalTime_Label;

        public override void Initialize()
        {
            base.Initialize();

            SubControls.Add(CurrentTime_TextBox);
            CurrentTime_TextBox.BorderWidth = 0;
            CurrentTime_TextBox.Skin.SetAllForegroundBlockID(ConcretePixel.ToBlockID(MinecraftColor.Orange));
            CurrentTime_TextBox.Skin.SetAllBackgroundBlockID(string.Empty);

            SubControls.Add(TotalTime_Label);
            TotalTime_Label.BorderWidth = 0;
            TotalTime_Label.Skin.SetAllForegroundBlockID(ConcretePixel.ToBlockID(MinecraftColor.Orange));
            TotalTime_Label.Skin.SetAllBackgroundBlockID(string.Empty);
        }

        public override void OnInitComplete3()
        {
            base.OnInitComplete3();

            AutoLayout();
        }

        private void VideoPlayer_OnVideoFrameUpdate(VideoFrame frame)
        {
            RequestUpdateFrame();
        }

        private void VideoPlayer__OnMediaFileUpdate(MediaFile? mediaFile)
        {
            TotalTime_Label.Text = '/' + VideoPlayer.FromTimeSpan(_owner.TotalTime);
            CurrentTime_TextBox.AutoSetSize();
        }

        public override Frame RenderingFrame()
        {
            AutoLayout();
            return base.RenderingFrame();
        }

        private void AutoLayout()
        {
            CurrentTime_TextBox.Text = VideoPlayer.FromTimeSpan(_owner.CurrentPosition);
            TotalTime_Label.AutoSetSize();

            TotalTime_Label.ClientLocation = this.RightLayout(CurrentTime_TextBox, 0);

            Width = CurrentTime_TextBox.Width + TotalTime_Label.Width;
        }
    }
}
