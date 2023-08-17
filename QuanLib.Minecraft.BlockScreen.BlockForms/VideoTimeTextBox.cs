using FFMediaToolkit.Decoding;
using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLib.Minecraft.BlockScreen.Frame;
using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.Block;
using QuanLib.Minecraft.BlockScreen.BlockForms.DialogBox;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public class VideoTimeTextBox : ContainerControl<Control>
    {
        public VideoTimeTextBox(VideoPlayerBox owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));

            CurrentTime_TextBox = new();
            TotalTime_Label = new();

            BorderWidth = 0;
            Skin.SetAllBackgroundBlockID(string.Empty);

            _owner.VideoBox.VideoFrameChanged += VideoPlayer_VideoFrameChanged;
            _owner.VideoBox.MediaFilePlayerChanged += VideoPlayer__MediaFileChanged;
        }

        protected readonly VideoPlayerBox _owner;

        private readonly TextBox CurrentTime_TextBox;

        private readonly Label TotalTime_Label;

        public override void Initialize()
        {
            base.Initialize();

            if (_owner != ParentContainer)
                throw new InvalidOperationException();

            SubControls.Add(CurrentTime_TextBox);
            CurrentTime_TextBox.BorderWidth = 0;
            CurrentTime_TextBox.AutoSize = true;
            CurrentTime_TextBox.Text = VideoPlayerBox.FromTimeSpan(_owner.VideoBox.CurrentPosition);
            CurrentTime_TextBox.Skin.SetAllForegroundBlockID(BlockManager.Concrete.Pink);
            CurrentTime_TextBox.Skin.BackgroundBlockID = string.Empty;
            CurrentTime_TextBox.TextEditorUpdate += CurrentTime_TextBox_TextEditorUpdate;

            SubControls.Add(TotalTime_Label);
            TotalTime_Label.BorderWidth = 0;
            TotalTime_Label.AutoSize = true;
            TotalTime_Label.Text = '/' + VideoPlayerBox.FromTimeSpan(_owner.VideoBox.TotalTime);
            TotalTime_Label.Skin.SetAllForegroundBlockID(BlockManager.Concrete.Pink);
            TotalTime_Label.Skin.SetAllBackgroundBlockID(string.Empty);
        }

        private void CurrentTime_TextBox_TextEditorUpdate(Control sender, CursorTextEventArgs e)
        {
            string timeText = e.Text;
            string[] times = timeText.Split(':');
            if (times.Length == 1)
            {
                timeText = "00:00:" + timeText;
            }
            else if (times.Length == 2)
            {
                timeText = "00:" + timeText;
            }

            if (_owner.VideoBox.MediaFilePlayer is null ||
                !TimeSpan.TryParse(timeText, out var time) ||
                !_owner.VideoBox.MediaFilePlayer.TryJumpToFrame(time))
            {
                Form? form = GetForm();
                if (form is not null)
                {
                    _ = DialogBoxHelper.OpenMessageBoxAsync(form, "警告", $"无法跳转到：“{e.Text}”", MessageBoxButtons.OK);
                }
            }
        }

        public override void OnInitCompleted3()
        {
            base.OnInitCompleted3();

            ActiveLayoutAll();
        }

        private void VideoPlayer_VideoFrameChanged(VideoBox sender, VideoFrameChangedEventArgs e)
        {
            RequestUpdateFrame();
            ActiveLayoutAll();
        }

        private void VideoPlayer__MediaFileChanged(VideoBox sender, MediaFilePlayerChangedEventArge e)
        {
            TotalTime_Label.Text = '/' + VideoPlayerBox.FromTimeSpan(_owner.VideoBox.TotalTime);
            ActiveLayoutAll();
        }

        public override IFrame RenderingFrame()
        {
            return base.RenderingFrame();
        }

        public override void ActiveLayoutAll()
        {
            CurrentTime_TextBox.Text = VideoPlayerBox.FromTimeSpan(_owner.VideoBox.CurrentPosition);
            TotalTime_Label.ClientLocation = this.RightLayout(CurrentTime_TextBox, 0);
            Width = CurrentTime_TextBox.Width + TotalTime_Label.Width;
        }
    }
}
