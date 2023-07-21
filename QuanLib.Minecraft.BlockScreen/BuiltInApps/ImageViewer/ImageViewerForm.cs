using QuanLib.Minecraft.BlockScreen.Controls;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BuiltInApps.ImageViewer
{
    public class ImageViewerForm : WindowForm
    {
        public ImageViewerForm()
        {
            Setting_Switch = new();
            Generate_Button = new();
            Path_TextBox = new();
            Picture_PictureBox = new();
            Setting_Panel = new();
            ResizeMode_ComboButton = new();
            AnchorPositionMode_ComboButton = new();
            Resampler_ComboButton = new();
        }

        private readonly Switch Setting_Switch;

        private readonly Button Generate_Button;

        private readonly TextBox Path_TextBox;

        private readonly PictureBox Picture_PictureBox;

        private readonly Panel<Control> Setting_Panel;

        private readonly ComboButton<ResizeMode> ResizeMode_ComboButton;

        private readonly ComboButton<AnchorPositionMode> AnchorPositionMode_ComboButton;

        private readonly ComboButton<IResampler> Resampler_ComboButton;

        public override void Initialize()
        {
            base.Initialize();

            Skin.BackgroundBlockID = ConcretePixel.ToBlockID(MinecraftColor.Lime);

            Client_Panel.OnResize += Client_FormPanel_OnResize;

            Client_Panel.SubControls.Add(Setting_Switch);
            Setting_Switch.OffText = "设置";
            Setting_Switch.OnText = "应用";
            Setting_Switch.Skin.BackgroundBlockID = Setting_Switch.Skin.BackgroundBlockID_Hover = ConcretePixel.ToBlockID(MinecraftColor.Lime);
            Setting_Switch.Skin.BackgroundBlockID_Selected = Setting_Switch.Skin.BackgroundBlockID_Hover_Selected = ConcretePixel.ToBlockID(MinecraftColor.Yellow);
            Setting_Switch.ClientLocation = Client_Panel.RightLayout(null, 2, 2);
            Setting_Switch.OnSelected += Setting_Switch_OnSelected;
            Setting_Switch.OnDeselected += Setting_Switch_OnDeselected;

            Client_Panel.SubControls.Add(Generate_Button);
            Generate_Button.Text = "生成";
            Generate_Button.ClientLocation = Client_Panel.LifeLayout(null, Generate_Button, 2, 2);
            Generate_Button.Anchor = PlaneFacing.Top | PlaneFacing.Right;
            Generate_Button.RightClick += Generate_Button_RightClick;

            Client_Panel.SubControls.Add(Path_TextBox);
            Path_TextBox.ClientLocation = Client_Panel.RightLayout(Setting_Switch, 2);
            Path_TextBox.Width = Client_Panel.ClientSize.Width - Setting_Switch.Width - Generate_Button.Width - 8;
            Path_TextBox.Stretch = PlaneFacing.Right;
            Path_TextBox.TextEditorUpdate += Path_TextBox_TextEditorUpdate;

            Client_Panel.SubControls.Add(Picture_PictureBox);
            Picture_PictureBox.ClientLocation = Client_Panel.BottomLayout(Setting_Switch, 2);
            Picture_PictureBox.ResizeOptions.Size = new(Client_Panel.ClientSize.Width - Picture_PictureBox.BorderWidth * 2 - 4, Client_Panel.ClientSize.Height - Picture_PictureBox.BorderWidth * 2 - Generate_Button.Height - 6);
            Picture_PictureBox.Stretch = PlaneFacing.Bottom | PlaneFacing.Right;

            Setting_Panel.ClientSize = new(128, 18 * 4 + 2);
            Setting_Panel.ClientLocation = Client_Panel.BottomLayout(Setting_Switch, 2);
            Setting_Panel.Skin.SetAllBackgroundBlockID(string.Empty);

            int width = Setting_Panel.ClientSize.Width - 4;

            Setting_Panel.SubControls.Add(ResizeMode_ComboButton);
            ResizeMode_ComboButton.Width = width;
            ResizeMode_ComboButton.ClientLocation = Setting_Panel.BottomLayout(null, 2, 2);
            ResizeMode_ComboButton.Skin.BackgroundBlockID = string.Empty;
            ResizeMode_ComboButton.Title = "模式";
            ResizeMode_ComboButton.Items.AddRenge(EnumUtil.ToArray<ResizeMode>());
            ResizeMode_ComboButton.Items.SelectedItem = PictureBox.DefaultResizeOptions.Mode;
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

            Setting_Panel.SubControls.Add(AnchorPositionMode_ComboButton);
            AnchorPositionMode_ComboButton.Width = width;
            AnchorPositionMode_ComboButton.ClientLocation = Setting_Panel.BottomLayout(ResizeMode_ComboButton, 2);
            AnchorPositionMode_ComboButton.Skin.BackgroundBlockID = string.Empty;
            AnchorPositionMode_ComboButton.Title = "锚点";
            AnchorPositionMode_ComboButton.Items.AddRenge(EnumUtil.ToArray<AnchorPositionMode>());
            AnchorPositionMode_ComboButton.Items.SelectedItem = PictureBox.DefaultResizeOptions.Position;
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

            Setting_Panel.SubControls.Add(Resampler_ComboButton);
            Resampler_ComboButton.Width= width;
            Resampler_ComboButton.ClientLocation = Setting_Panel.BottomLayout(AnchorPositionMode_ComboButton, 2);
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
            Resampler_ComboButton.Items.SelectedItem = PictureBox.DefaultResizeOptions.Sampler;

            foreach (var control in Setting_Panel.SubControls)
            {
                control.Initialize();
            }
        }

        private void Client_FormPanel_OnResize(Size oldSize, Size newSize)
        {
            if (Picture_PictureBox.Image is null)
            {
                Picture_PictureBox.ResizeOptions.Size = newSize;
            }
            else
            {
                Picture_PictureBox.Image.ResizeOptions.Size = Picture_PictureBox.ClientSize;
                Picture_PictureBox.Image.Update();
            }
        }

        private void Setting_Switch_OnSelected()
        {
            Client_Panel.SubControls.Add(Setting_Panel);
        }

        private void Setting_Switch_OnDeselected()
        {
            ResizeOptions options;
            if (Picture_PictureBox.Image is null)
                options = Picture_PictureBox.ResizeOptions;
            else
                options = Picture_PictureBox.Image.ResizeOptions;

            options.Mode = ResizeMode_ComboButton.Items.SelectedItem;
            options.Position = AnchorPositionMode_ComboButton.Items.SelectedItem;
            options.Sampler = Resampler_ComboButton.Items.SelectedItem ?? PictureBox.DefaultResizeOptions.Sampler;
            Client_Panel.SubControls.Remove(Setting_Panel);
        }

        private void Path_TextBox_TextEditorUpdate(Point position, string text)
        {
            if (MCOS.DefaultFont.GetTotalSize(text).Width > Path_TextBox.ClientSize.Width)
                Path_TextBox.ContentLayout = ContentLayout.UpperRight;
            else
                Path_TextBox.ContentLayout = ContentLayout.UpperLeft;
        }

        private void Generate_Button_RightClick(Point position)
        {
            if (!Picture_PictureBox.TryReadImageFile(Path_TextBox.Text))
            {
                Path_TextBox.Text = "生成失败";
                Picture_PictureBox.Image = null;
            }
        }
    }
}
