using QuanLib.Minecraft.Block;
using QuanLib.Minecraft.BlockScreen.BlockForms;
using QuanLib.Minecraft.BlockScreen.BlockForms.DialogBox;
using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using QuanLib.Minecraft.BlockScreen.Event;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.Album
{
    public class AlbumForm : WindowForm
    {
        public AlbumForm(string? path = null)
        {
            _open = path;
            _extensions = new()
            {
                "jpg",
                "jpeg",
                "png",
                "bmp",
                "webp"
            };

            Setting_Switch = new();
            Path_TextBox = new();
            PreviousImage_Button = new();
            NextImage_Button = new();
            ScalablePictureBox = new();
            Setting_ListMenuBox = new();
            ResizeMode_ComboButton = new();
            AnchorPositionMode_ComboButton = new();
            Resampler_ComboButton = new();

            OverlayShowTime = 20;
            OverlayHideTime = 0;
        }

        private readonly string? _open;

        private readonly List<string> _extensions;

        private FileList? _images;

        private readonly ScalablePictureBox ScalablePictureBox;

        private readonly Switch Setting_Switch;

        private readonly TextBox Path_TextBox;

        private readonly Button PreviousImage_Button;

        private readonly Button NextImage_Button;

        private readonly ListMenuBox<Control> Setting_ListMenuBox;

        private readonly ComboButton<ResizeMode> ResizeMode_ComboButton;

        private readonly ComboButton<AnchorPositionMode> AnchorPositionMode_ComboButton;

        private readonly ComboButton<IResampler> Resampler_ComboButton;

        public int OverlayShowTime { get; set; }

        public int OverlayHideTime { get; set; }

        public override void Initialize()
        {
            base.Initialize();

            ClientPanel.SubControls.Add(ScalablePictureBox);
            ScalablePictureBox.EnableZoom = true;
            ScalablePictureBox.EnableDrag = true;
            ScalablePictureBox.BorderWidth = 0;
            ScalablePictureBox.ClientSize = ClientPanel.ClientSize;
            ScalablePictureBox.Stretch = Direction.Bottom | Direction.Right;
            ScalablePictureBox.Resize += ScalablePictureBox_Resize;

            ClientPanel.SubControls.Add(Setting_Switch);
            Setting_Switch.OffText = "设置";
            Setting_Switch.OnText = "应用";
            Setting_Switch.Skin.BackgroundBlockID = string.Empty;
            Setting_Switch.Skin.BackgroundBlockID_Hover = BlockManager.Concrete.Yellow;
            Setting_Switch.Skin.BackgroundBlockID_Selected = Setting_Switch.Skin.BackgroundBlockID_Hover_Selected = BlockManager.Concrete.Orange;
            Setting_Switch.ClientLocation = new(2, 2);
            Setting_Switch.ControlSelected += Setting_Switch_ControlSelected;
            Setting_Switch.ControlDeselected += Setting_Switch_ControlDeselected;

            ClientPanel.SubControls.Add(Path_TextBox);
            Path_TextBox.ClientLocation = ClientPanel.RightLayout(Setting_Switch, 2);
            Path_TextBox.Width = ClientPanel.ClientSize.Width - Setting_Switch.Width - 6;
            Path_TextBox.Stretch = Direction.Right;
            Path_TextBox.Skin.BackgroundBlockID = string.Empty;
            Path_TextBox.TextChanged += Path_TextBox_TextChanged;

            ClientPanel.SubControls.Add(PreviousImage_Button);
            PreviousImage_Button.Text = "<";
            PreviousImage_Button.ClientSize = new(16, 16);
            PreviousImage_Button.LayoutSyncer = new(ClientPanel, (sender, e) => { }, (sender, e) =>
            PreviousImage_Button.ClientLocation = ClientPanel.VerticalCenterLayout(PreviousImage_Button, 2));
            PreviousImage_Button.Skin.BackgroundBlockID = string.Empty;
            PreviousImage_Button.RightClick += PreviousImage_Button_RightClick;

            ClientPanel.SubControls.Add(NextImage_Button);
            NextImage_Button.Text = ">";
            NextImage_Button.ClientSize = new(16, 16);
            NextImage_Button.LayoutSyncer = new(ClientPanel, (sender, e) => { }, (sender, e) =>
            NextImage_Button.ClientLocation = ClientPanel.VerticalCenterLayout(NextImage_Button, ClientPanel.ClientSize.Width - NextImage_Button.Width - 3));
            NextImage_Button.Skin.BackgroundBlockID = string.Empty;
            NextImage_Button.RightClick += NextImage_Button_RightClick;

            Setting_ListMenuBox.ClientSize = new(128, 20 * 3 + 2);
            Setting_ListMenuBox.Spacing = 2;
            Setting_ListMenuBox.ClientLocation = ClientPanel.BottomLayout(Setting_Switch, 2);
            Setting_ListMenuBox.Skin.SetAllBackgroundBlockID(string.Empty);

            int width = Setting_ListMenuBox.ClientSize.Width - 4;

            ResizeMode_ComboButton.Width = width;
            ResizeMode_ComboButton.Skin.BackgroundBlockID = string.Empty;
            ResizeMode_ComboButton.Title = "模式";
            ResizeMode_ComboButton.Items.AddRenge(EnumUtil.ToArray<ResizeMode>());
            ResizeMode_ComboButton.Items.SelectedItem = ScalablePictureBox.DefaultResizeOptions.Mode;
            ResizeMode_ComboButton.Items.ItemToStringFunc = (item) =>
            {
                return item switch
                {
                    ResizeMode.Crop => "裁剪",
                    ResizeMode.Pad => "填充",
                    ResizeMode.BoxPad => "盒式填充",
                    ResizeMode.Max => "最大",
                    ResizeMode.Min => "最小",
                    ResizeMode.Stretch => "拉伸",
                    ResizeMode.Manual => "手动",
                    _ => throw new InvalidOperationException(),
                };
            };
            Setting_ListMenuBox.AddedSubControlAndLayout(ResizeMode_ComboButton);

            AnchorPositionMode_ComboButton.Width = width;
            AnchorPositionMode_ComboButton.Skin.BackgroundBlockID = string.Empty;
            AnchorPositionMode_ComboButton.Title = "锚点";
            AnchorPositionMode_ComboButton.Items.AddRenge(EnumUtil.ToArray<AnchorPositionMode>());
            AnchorPositionMode_ComboButton.Items.SelectedItem = ScalablePictureBox.DefaultResizeOptions.Position;
            AnchorPositionMode_ComboButton.Items.ItemToStringFunc = (item) =>
            {
                return item switch
                {
                    AnchorPositionMode.Center => "中心",
                    AnchorPositionMode.Top => "顶部",
                    AnchorPositionMode.Bottom => "底部",
                    AnchorPositionMode.Left => "左侧",
                    AnchorPositionMode.Right => "右侧",
                    AnchorPositionMode.TopLeft => "左上角",
                    AnchorPositionMode.TopRight => "右上角",
                    AnchorPositionMode.BottomRight => "右下角",
                    AnchorPositionMode.BottomLeft => "左下角",
                    _ => throw new InvalidOperationException(),
                };
            };
            Setting_ListMenuBox.AddedSubControlAndLayout(AnchorPositionMode_ComboButton);

            Resampler_ComboButton.Width = width;
            Resampler_ComboButton.Skin.BackgroundBlockID = string.Empty;
            Resampler_ComboButton.Title = "算法";
            Resampler_ComboButton.Items.Add(KnownResamplers.Bicubic, nameof(KnownResamplers.Bicubic));
            Resampler_ComboButton.Items.Add(KnownResamplers.Box, nameof(KnownResamplers.Box));
            Resampler_ComboButton.Items.Add(KnownResamplers.CatmullRom, nameof(KnownResamplers.CatmullRom));
            Resampler_ComboButton.Items.Add(KnownResamplers.Hermite, nameof(KnownResamplers.Hermite));
            Resampler_ComboButton.Items.Add(KnownResamplers.Lanczos2, nameof(KnownResamplers.Lanczos2));
            Resampler_ComboButton.Items.Add(KnownResamplers.Lanczos3, nameof(KnownResamplers.Lanczos3));
            Resampler_ComboButton.Items.Add(KnownResamplers.Lanczos5, nameof(KnownResamplers.Lanczos5));
            Resampler_ComboButton.Items.Add(KnownResamplers.Lanczos8, nameof(KnownResamplers.Lanczos8));
            Resampler_ComboButton.Items.Add(KnownResamplers.MitchellNetravali, nameof(KnownResamplers.MitchellNetravali));
            Resampler_ComboButton.Items.Add(KnownResamplers.NearestNeighbor, nameof(KnownResamplers.NearestNeighbor));
            Resampler_ComboButton.Items.Add(KnownResamplers.Robidoux, nameof(KnownResamplers.Robidoux));
            Resampler_ComboButton.Items.Add(KnownResamplers.RobidouxSharp, nameof(KnownResamplers.RobidouxSharp));
            Resampler_ComboButton.Items.Add(KnownResamplers.Spline, nameof(KnownResamplers.Spline));
            Resampler_ComboButton.Items.SelectedItem = ScalablePictureBox.DefaultResizeOptions.Sampler;
            Setting_ListMenuBox.AddedSubControlAndLayout(Resampler_ComboButton);

            if (_open is not null)
                Path_TextBox.Text = _open;
            else
                ScalablePictureBox.SetImage(ScalablePictureBox.CreateImage(ScalablePictureBox.ClientSize, BlockManager.Concrete.White));
        }

        protected override void OnCursorMove(Control sender, CursorEventArgs e)
        {
            base.OnCursorMove(sender, e);

            ShowOverlay();
            OverlayHideTime = OverlayShowTime;
        }

        protected override void OnBeforeFrame(Control sender, EventArgs e)
        {
            base.OnBeforeFrame(sender, e);

            if (ClientPanel.SubControls.FirstHover is null or BlockForms.ScalablePictureBox)
            {
                if (OverlayHideTime <= 0)
                    HideOverlay();
                OverlayHideTime--;
            }
        }

        private void ShowOverlay()
        {
            Setting_Switch.Visible = true;
            Path_TextBox.Visible = true;
            PreviousImage_Button.Visible = true;
            NextImage_Button.Visible = true;
        }

        private void HideOverlay()
        {
            Setting_Switch.Visible = false;
            Path_TextBox.Visible = false;
            PreviousImage_Button.Visible = false;
            NextImage_Button.Visible = false;
        }

        private void ScalablePictureBox_Resize(Control sender, SizeChangedEventArgs e)
        {
            ScalablePictureBox.ClientLocation = ClientPanel.CenterLayout(ScalablePictureBox);
        }

        private void Setting_Switch_ControlSelected(Control sender, EventArgs e)
        {
            ClientPanel.SubControls.TryAdd(Setting_ListMenuBox);
        }

        private void Setting_Switch_ControlDeselected(Control sender, EventArgs e)
        {
            ApplySetting(ScalablePictureBox.DefaultResizeOptions);
            ApplySetting(ScalablePictureBox.ImageFrame.ResizeOptions);
            ScalablePictureBox.ImageFrame.Update(ScalablePictureBox.Rectangle);
            ScalablePictureBox.AutoSetSize();
            ClientPanel.SubControls.Remove(Setting_ListMenuBox);
        }

        private void Path_TextBox_TextChanged(Control sender, QuanLib.Event.TextChangedEventArgs e)
        {
            if (SystemResourcesManager.DefaultFont.GetTotalSize(e.NewText).Width > Path_TextBox.ClientSize.Width)
                Path_TextBox.ContentAnchor = AnchorPosition.UpperRight;
            else
                Path_TextBox.ContentAnchor = AnchorPosition.UpperLeft;

            if (ScalablePictureBox.TryReadImageFile(e.NewText))
            {
                if (_images is null || !_images.Contains(e.NewText))
                    _images = FileList.LoadFile(e.NewText, _extensions);
            }
            else
            {
                _ = DialogBoxManager.OpenMessageBoxAsync(this, "警告", $"无法打开图片文件：“{e.NewText}”", MessageBoxButtons.OK);
            }
        }

        private void PreviousImage_Button_RightClick(Control sender, CursorEventArgs e)
        {
            string? file = _images?.GetPrevious();
            if (file is not null)
                Path_TextBox.Text = file;
        }

        private void NextImage_Button_RightClick(Control sender, CursorEventArgs e)
        {
            string? file = _images?.GetNext();
            if (file is not null)
                Path_TextBox.Text = file;
        }

        private void ApplySetting(ResizeOptions options)
        {
            options.Mode = ResizeMode_ComboButton.Items.SelectedItem;
            options.Position = AnchorPositionMode_ComboButton.Items.SelectedItem;
            options.Sampler = Resampler_ComboButton.Items.SelectedItem ?? ImageFrame.DefaultResizeOptions.Sampler;
        }
    }
}
