using QuanLib.Minecraft.BlockScreen.Frame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public class HorizontalScrollBar : ScrollBar
    {
        public override IFrame RenderingFrame()
        {
            int slider = (int)Math.Round(ClientSize.Width * SliderSize);
            int position = (int)Math.Round(ClientSize.Width * SliderPosition);

            ArrayFrame frame = ArrayFrame.BuildFrame(ClientSize.Width, ClientSize.Height, Skin.GetBackgroundBlockID());
            frame.Overwrite(ArrayFrame.BuildFrame(slider, ClientSize.Height, Skin.GetForegroundBlockID()), new(position, 0));

            return frame;
        }
    }
}
