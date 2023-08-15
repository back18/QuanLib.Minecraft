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
    public class VideoPlayerBox : ContainerControl<Control>
    {
        public VideoPlayerBox()
        {
            VideoBox = new();
            PauseOrResume_Switch = new();
            ProgressBar_VideoProgressBar = new(this);
            TimeText_VideoTimeTextBox = new(this);

            OverlayShowTime = 20;
            OverlayHideTime = 0;
        }

        public readonly VideoBox VideoBox;

        private readonly Switch PauseOrResume_Switch;

        private readonly VideoProgressBar ProgressBar_VideoProgressBar;

        private readonly VideoTimeTextBox TimeText_VideoTimeTextBox;

        public int OverlayShowTime { get; set; }

        public int OverlayHideTime { get; private set; }

        public override void Initialize()
        {
            base.Initialize();

            SubControls.Add(VideoBox);
            VideoBox.BorderWidth = 0;
            VideoBox.DisplayPriority = -32;
            VideoBox.MaxDisplayPriority = -16;
            VideoBox.ClientSize = ClientSize;
            VideoBox.Stretch = Direction.Bottom | Direction.Right;
            VideoBox.Played += VideoBox_Played;
            VideoBox.Paused += VideoBox_Paused;

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

            SubControls.Add(PauseOrResume_Switch);
            PauseOrResume_Switch.Visible = false;
            PauseOrResume_Switch.BorderWidth = 0;
            PauseOrResume_Switch.ClientSize = new(16, 16);
            PauseOrResume_Switch.ClientLocation = this.LeftLayout(null, PauseOrResume_Switch, 2, TimeText_VideoTimeTextBox.TopLocation);
            PauseOrResume_Switch.Anchor = Direction.Bottom | Direction.Right;
            PauseOrResume_Switch.Skin.SetBackgroundImage(ControlState.None, TextureManager.GetTexture("Play"));
            PauseOrResume_Switch.Skin.SetBackgroundImage(ControlState.Hover, TextureManager.GetTexture("Play"));
            PauseOrResume_Switch.Skin.SetBackgroundImage(ControlState.Selected, TextureManager.GetTexture("Pause"));
            PauseOrResume_Switch.Skin.SetBackgroundImage(ControlState.Hover | ControlState.Selected, TextureManager.GetTexture("Pause"));
            PauseOrResume_Switch.IsSelected = false;
            PauseOrResume_Switch.ControlSelected += PauseOrResume_Switch_OnSelected;
            PauseOrResume_Switch.ControlDeselected += PauseOrResume_Switch_ControlDeselected;
        }

        protected override void OnCursorMove(Control sender, CursorEventArgs e)
        {
            base.OnCursorMove(sender, e);

            ShowOverlay();
            OverlayHideTime = OverlayShowTime;
        }

        protected override void OnRightClick(Control sender, CursorEventArgs e)
        {
            base.OnRightClick(sender, e);

            if (SubControls.FirstHover is null or BlockForms.VideoBox)
            {
                if (PauseOrResume_Switch.Visible)
                {
                    HideOverlay();
                    OverlayHideTime = 0;
                }
                else
                {
                    ShowOverlay();
                    OverlayHideTime = OverlayShowTime;
                }
            }
        }

        protected override void OnBeforeFrame(Control sender, EventArgs e)
        {
            base.OnBeforeFrame(sender, e);

            if (SubControls.FirstHover is null or BlockForms.VideoBox)
            {
                if (OverlayHideTime <= 0)
                    HideOverlay();
                OverlayHideTime--;
            }
        }

        private void ShowOverlay()
        {
            PauseOrResume_Switch.Visible = true;
            ProgressBar_VideoProgressBar.Visible = true;
            TimeText_VideoTimeTextBox.Visible = true;
        }

        private void HideOverlay()
        {
            PauseOrResume_Switch.Visible = false;
            ProgressBar_VideoProgressBar.Visible = false;
            TimeText_VideoTimeTextBox.Visible = false;
        }

        private void VideoBox_Played(VideoBox sender, EventArgs e)
        {
            PauseOrResume_Switch.IsSelected = true;
        }

        private void VideoBox_Paused(VideoBox sender, EventArgs e)
        {
            PauseOrResume_Switch.IsSelected = false;
        }

        private void PauseOrResume_Switch_OnSelected(Control sender, EventArgs e)
        {
            VideoBox.MediaFilePlayer?.Play();
        }

        private void PauseOrResume_Switch_ControlDeselected(Control sender, EventArgs e)
        {
            VideoBox.MediaFilePlayer?.Pause();
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
