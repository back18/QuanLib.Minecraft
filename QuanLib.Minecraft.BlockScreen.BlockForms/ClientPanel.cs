using QuanLib.Minecraft.BlockScreen.Frame;
using QuanLib.Minecraft.BlockScreen.UI;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageFrame = QuanLib.Minecraft.BlockScreen.Frame.ImageFrame;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public class ClientPanel : ScrollablePanel
    {
        public ClientPanel()
        {
            BorderWidth = 0;
        }

        public override IFrame RenderingFrame()
        {
            ImageFrame? image = Skin.GetBackgroundImage();
            if (image is null)
                return base.RenderingFrame();

            Size size = GetRenderingSize();
            if (image.FrameSize != size)
            {
                image.ResizeOptions.Size = size;
                image.Update();
            }

            return image.GetFrameClone();
        }
    }
}
