using QuanLib.Minecraft.BlockScreen.UI;
using QuanLib.Minecraft.BlockScreen.BlockForms;
using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.Block;
using QuanLib.Event;
using QuanLib.Minecraft.BlockScreen.BlockForms.DialogBox;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.VideoPlayer
{
    public class VideoPlayerForm : WindowForm
    {
        public VideoPlayerForm(string? path = null)
        {
            _open = path;

            VideoPlayer = new();
            Setting_Switch = new();
            Path_TextBox = new();

            OverlayShowTime = 20;
            OverlayHideTime = 0;
        }

        private readonly string? _open;

        private readonly VideoPlayerBox VideoPlayer;

        private readonly Switch Setting_Switch;

        private readonly TextBox Path_TextBox;

        public int OverlayShowTime { get; set; }

        public int OverlayHideTime { get; set; }

        public override void Initialize()
        {
            base.Initialize();

            ClientPanel.SubControls.Add(VideoPlayer);
            VideoPlayer.BorderWidth = 0;
            VideoPlayer.Size = ClientPanel.ClientSize;
            VideoPlayer.Stretch = Direction.Bottom | Direction.Right;

            ClientPanel.SubControls.Add(Setting_Switch);
            Setting_Switch.OffText = "设置";
            Setting_Switch.OnText = "应用";
            Setting_Switch.Skin.SetAllForegroundBlockID(BlockManager.Concrete.Pink);
            Setting_Switch.Skin.BackgroundBlockID = string.Empty;
            Setting_Switch.Skin.BackgroundBlockID_Hover = BlockManager.Concrete.LightBlue;
            Setting_Switch.Skin.BackgroundBlockID_Selected = Setting_Switch.Skin.BackgroundBlockID_Hover_Selected = BlockManager.Concrete.Lime;
            Setting_Switch.ClientLocation = new(2, 2);

            ClientPanel.SubControls.Add(Path_TextBox);
            Path_TextBox.ClientLocation = ClientPanel.RightLayout(Setting_Switch, 2);
            Path_TextBox.Width = ClientPanel.ClientSize.Width - Setting_Switch.Width - 6;
            Path_TextBox.Stretch = Direction.Right;
            Path_TextBox.Skin.SetAllForegroundBlockID(BlockManager.Concrete.Pink);
            Path_TextBox.Skin.BackgroundBlockID = string.Empty;
            Path_TextBox.TextChanged += Path_TextBox_TextChanged;
        }

        public override void OnInitCompleted3()
        {
            base.OnInitCompleted3();

            if (_open is not null)
                Path_TextBox.Text = _open;
        }

        protected override void OnCursorMove(Control sender, CursorEventArgs e)
        {
            base.OnCursorMove(sender, e);

            ShowOverlay();
            OverlayHideTime = OverlayShowTime;
        }

        protected override void OnRightClick(Control sender, CursorEventArgs e)
        {
            if (ClientPanel.SubControls.FirstHover is null or VideoPlayerBox)
            {
                if (Setting_Switch.Visible)
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

            if (ClientPanel.SubControls.FirstHover is null or VideoPlayerBox)
            {
                if (OverlayHideTime <= 0)
                    HideOverlay();
                OverlayHideTime--;
            }
        }

        protected override void OnFormClose(IForm sender, EventArgs e)
        {
            base.OnFormClose(sender, e);

            VideoPlayer.VideoBox.Dispose();
        }

        private void ShowOverlay()
        {
            Setting_Switch.Visible = true;
            Path_TextBox.Visible = true;
        }

        private void HideOverlay()
        {
            Setting_Switch.Visible = false;
            Path_TextBox.Visible = false;
        }

        private void Path_TextBox_TextChanged(Control sender, TextChangedEventArgs e)
        {
            if (SystemResourcesManager.DefaultFont.GetTotalSize(e.NewText).Width > Path_TextBox.ClientSize.Width)
                Path_TextBox.ContentAnchor = AnchorPosition.UpperRight;
            else
                Path_TextBox.ContentAnchor = AnchorPosition.UpperLeft;

            if (VideoPlayer.VideoBox.TryReadMediaFile(e.NewText))
            {
                VideoPlayer.VideoBox.MediaFilePlayer?.Play();
            }
            else
            {
                _ = DialogBoxManager.OpenMessageBoxAsync(this, "警告", $"无法打开视频文件：“{e.NewText}”", MessageBoxButtons.OK);
            }
        }
    }
}
