using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Controls
{
    public class IconTextControl : ContainerControl<Control>
    {
        public IconTextControl()
        {
            Image_PictureBox = new();
            Text_Label = new();

            ClientSize = new(MCOS.DefaultFont.HalfWidth * 6, MCOS.DefaultFont.Height);
        }

        private readonly PictureBox Image_PictureBox;

        private readonly Label Text_Label;

        public ImageFrame? Image
        {
            get => Image_PictureBox.Image;
            set
            {
                if (Image != value)
                {
                    Image_PictureBox.Image = value;
                    RequestUpdateFrame();
                }
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            ActiveLayoutAll();
        }

        public override void ActiveLayoutAll()
        {
            Image_PictureBox.ClientLocation = this.HorizontalCenterLayout(Image_PictureBox, 0);
            Text_Label.ClientLocation = this.HorizontalCenterLayout(Text_Label, Image_PictureBox.RightLocation + 1);
        }

        public override void AutoSetSize()
        {
            Size size = MCOS.DefaultFont.GetTotalSize(Text);
            if (Image is not null)
            {
                size.Width += Image.FrameSize.Width;
                if (Image.FrameSize.Height > size.Height)
                {
                    size.Height = Image.FrameSize.Height;
                }
            }
            ClientSize = size;
        }
    }
}
