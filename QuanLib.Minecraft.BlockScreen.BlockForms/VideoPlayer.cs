using QuanLib.Minecraft.BlockScreen.UI;
using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLib.Minecraft.BlockScreen.Event;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public class VideoPlayer : ContainerControl<Control>
    {
        public VideoPlayer()
        {
            VideoBox = new();
            PauseOrResume_Switch = new();
            ProgressBar_VideoProgressBar = new(this);
            TimeText_VideoTimeTextBox = new(this);

            OverlayLayerShowTime = 20;
            OverlayLayerCountdown = 0;
        }

        public readonly VideoBox VideoBox;

        private readonly Switch PauseOrResume_Switch;

        private readonly VideoProgressBar ProgressBar_VideoProgressBar;

        private readonly VideoTimeTextBox TimeText_VideoTimeTextBox;

        public int OverlayLayerShowTime { get; set; }

        public int OverlayLayerCountdown { get; private set; }

        public override void Initialize()
        {
            base.Initialize();

            MCOS os = MCOS.GetMCOS();
            string dir = PathManager.SystemResources_Textures_Control_Dir;

            SubControls.Add(VideoBox);
            VideoBox.BorderWidth = 0;
            VideoBox.DisplayPriority = -32;
            VideoBox.MaxDisplayPriority = -16;
            VideoBox.Resume += VideoBox_Resume;
            VideoBox.Pause += VideoBox_Pause;
            VideoBox.EndedPlay += VideoBox_EndedPlay;

            SubControls.Add(PauseOrResume_Switch);
            PauseOrResume_Switch.Visible = false;
            PauseOrResume_Switch.BorderWidth = 0;
            PauseOrResume_Switch.ClientSize = new(16, 16);
            PauseOrResume_Switch.ClientLocation = this.CenterLayout(PauseOrResume_Switch);
            PauseOrResume_Switch.LayoutSyncer = new(this, (sender, e) => { }, (sender, e) =>
            PauseOrResume_Switch.ClientLocation = this.CenterLayout(PauseOrResume_Switch));
            PauseOrResume_Switch.Skin.SetBackgroundImage(ControlState.None, TextureManager.GetTexture("Pause"));
            PauseOrResume_Switch.Skin.SetBackgroundImage(ControlState.Hover, TextureManager.GetTexture("Pause"));
            PauseOrResume_Switch.Skin.SetBackgroundImage(ControlState.Selected, TextureManager.GetTexture("Play"));
            PauseOrResume_Switch.Skin.SetBackgroundImage(ControlState.Hover | ControlState.Selected, TextureManager.GetTexture("Play"));
            PauseOrResume_Switch.IsSelected = VideoBox.PlayerState == VideoPlayerState.Playing;
            PauseOrResume_Switch.ControlSelected += PauseOrResume_Switch_OnSelected;
            PauseOrResume_Switch.ControlDeselected += PauseOrResume_Switch_ControlDeselected;

            SubControls.Add(ProgressBar_VideoProgressBar);
            ProgressBar_VideoProgressBar.Visible = false;
            ProgressBar_VideoProgressBar.Height = 6;
            ProgressBar_VideoProgressBar.Width = ClientSize.Width - 4;
            ProgressBar_VideoProgressBar.ClientLocation = this.TopLayout(null, ProgressBar_VideoProgressBar, 2, 2);
            ProgressBar_VideoProgressBar.Anchor = Direction.Bottom | Direction.Left;
            ProgressBar_VideoProgressBar.Stretch = Direction.Left;

            SubControls.Add(TimeText_VideoTimeTextBox);
            TimeText_VideoTimeTextBox.Visible = false;
            TimeText_VideoTimeTextBox.ClientLocation = this.TopLayout(ProgressBar_VideoProgressBar, TimeText_VideoTimeTextBox, 2);
            TimeText_VideoTimeTextBox.Anchor = Direction.Bottom | Direction.Left;
        }

        protected override void OnCursorMove(Control sender, CursorEventArgs e)
        {
            base.OnCursorMove(sender, e);

            IsSelected = true;
            OverlayLayerCountdown = OverlayLayerShowTime;
        }

        protected override void OnRightClick(Control sender, CursorEventArgs e)
        {
            base.OnRightClick(sender, e);

            if (IsSelected && !SubControls.HaveHover)
            {
                IsSelected = false;
                OverlayLayerCountdown = 0;
            }
            else
            {
                IsSelected = true;
                OverlayLayerCountdown = OverlayLayerShowTime;
            }
        }

        protected override void OnControlSelected(Control sender, EventArgs e)
        {
            base.OnControlSelected(sender, e);

            PauseOrResume_Switch.Visible = true;
            ProgressBar_VideoProgressBar.Visible = true;
            TimeText_VideoTimeTextBox.Visible = true;
        }

        protected override void OnControlDeselected(Control sender, EventArgs e)
        {
            base.OnControlDeselected(sender, e);

            PauseOrResume_Switch.Visible = false;
            ProgressBar_VideoProgressBar.Visible = false;
            TimeText_VideoTimeTextBox.Visible = false;
        }

        protected override void OnBeforeFrame(Control sender, EventArgs e)
        {
            base.OnBeforeFrame(sender, e);

            if (IsSelected && !SubControls.HaveHover)
            {
                if (OverlayLayerCountdown <= 0)
                    IsSelected = false;
                OverlayLayerCountdown--;
            }
        }

        private void VideoBox_Resume(VideoBox sender, EventArgs e)
        {
            PauseOrResume_Switch.IsSelected = true;
        }

        private void VideoBox_Pause(VideoBox sender, EventArgs e)
        {
            PauseOrResume_Switch.IsSelected = false;
        }

        private void VideoBox_EndedPlay(VideoBox sender, EventArgs e)
        {
            PauseOrResume_Switch.IsSelected = false;
        }

        private void PauseOrResume_Switch_OnSelected(Control sender, EventArgs e)
        {
            VideoBox.Resumeing();
        }

        private void PauseOrResume_Switch_ControlDeselected(Control sender, EventArgs e)
        {
            VideoBox.Pauseing();
        }

        public static string FromTimeSpan(TimeSpan time)
        {
            string result = string.Empty;
            if (time.Hours > 0)
                result += time.Hours.ToString().PadLeft(2, '0') + ':';
            result += $"{time.Minutes.ToString().PadLeft(2, '0')}:{time.Seconds.ToString().PadLeft(2, '0')}";
            return result;
        }
    }
}
