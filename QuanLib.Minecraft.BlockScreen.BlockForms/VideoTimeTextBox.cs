using FFMediaToolkit.Decoding;
using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLib.Minecraft.BlockScreen.Frame;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public class VideoTimeTextBox : ContainerControl<Control>
    {
        public VideoTimeTextBox(VideoPlayer owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));

            CurrentTime_TextBox = new();
            TotalTime_Label = new();

            BorderWidth = 0;
            Skin.SetAllBackgroundBlockID(string.Empty);

            _owner.VideoBox.OnVideoFrameUpdate += VideoPlayer_OnVideoFrameUpdate;
            _owner.VideoBox.OnMediaFileUpdate += VideoPlayer__OnMediaFileUpdate;
        }

        protected readonly VideoPlayer _owner;

        private readonly TextBox CurrentTime_TextBox;

        private readonly Label TotalTime_Label;

        public override void Initialize()
        {
            base.Initialize();

            if (_owner != ParentContainer)
                throw new InvalidOperationException();

            SubControls.Add(CurrentTime_TextBox);
            CurrentTime_TextBox.BorderWidth = 0;
            CurrentTime_TextBox.Skin.SetAllForegroundBlockID(ConcretePixel.ToBlockID(MinecraftColor.Orange));
            CurrentTime_TextBox.Skin.SetAllBackgroundBlockID(string.Empty);

            SubControls.Add(TotalTime_Label);
            TotalTime_Label.BorderWidth = 0;
            TotalTime_Label.Skin.SetAllForegroundBlockID(ConcretePixel.ToBlockID(MinecraftColor.Orange));
            TotalTime_Label.Skin.SetAllBackgroundBlockID(string.Empty);
        }

        public override void OnInitCompleted3()
        {
            base.OnInitCompleted3();

            AutoLayout();
        }

        private void VideoPlayer_OnVideoFrameUpdate(VideoFrame frame)
        {
            RequestUpdateFrame();
        }

        private void VideoPlayer__OnMediaFileUpdate(MediaFile? mediaFile)
        {
            TotalTime_Label.Text = '/' + VideoPlayer.FromTimeSpan(_owner.VideoBox.TotalTime);
            CurrentTime_TextBox.AutoSetSize();
        }

        public override IFrame RenderingFrame()
        {
            AutoLayout();
            return base.RenderingFrame();
        }

        private void AutoLayout()
        {
            CurrentTime_TextBox.Text = VideoPlayer.FromTimeSpan(_owner.VideoBox.CurrentPosition);
            TotalTime_Label.AutoSetSize();

            TotalTime_Label.ClientLocation = this.RightLayout(CurrentTime_TextBox, 0);

            Width = CurrentTime_TextBox.Width + TotalTime_Label.Width;
        }
    }
}
