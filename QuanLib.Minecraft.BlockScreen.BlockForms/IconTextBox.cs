using QuanLib.Event;
using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using QuanLib.Minecraft.BlockScreen.Event;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public class IconTextBox : ContainerControl<Control>
    {
        public IconTextBox()
        {
            Image_PictureBox = new();
            Text_Label = new();

            ClientSize = new(SystemResourcesManager.DefaultFont.HalfWidth * 6, SystemResourcesManager.DefaultFont.Height);
        }

        private readonly PictureBox Image_PictureBox;

        private readonly Label Text_Label;

        public ImageFrame? Icon
        {
            get => Image_PictureBox.ImageFrame;
            set
            {
                if (Icon != value)
                {
                    Image_PictureBox.ImageFrame = value;
                    RequestUpdateFrame();
                }
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            SubControls.Add(Image_PictureBox);
            Image_PictureBox.BorderWidth = 0;
            Image_PictureBox.DefaultResizeOptions.Size = new(16, 16);

            SubControls.Add(Text_Label);

            ActiveLayoutAll();
        }

        protected override void OnTextChanged(Control sender, TextChangedEventArgs e)
        {
            base.OnTextChanged(sender, e);

            Text_Label.Text = e.NewText;
        }

        public override void ActiveLayoutAll()
        {
            Image_PictureBox.ClientLocation = this.VerticalCenterLayout(Image_PictureBox, 0);
            Text_Label.ClientLocation = this.VerticalCenterLayout(Text_Label, Image_PictureBox.RightLocation + 1);
        }

        public override void AutoSetSize()
        {
            Size size = SystemResourcesManager.DefaultFont.GetTotalSize(Text);
            if (Icon is not null)
            {
                size.Width += Icon.FrameSize.Width;
                if (Icon.FrameSize.Height > size.Height)
                {
                    size.Height = Icon.FrameSize.Height;
                }
            }
            ClientSize = size;
        }
    }
}
