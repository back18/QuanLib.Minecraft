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
            int position = (int)Math.Round(ClientSize.Width * SliderPosition);
            int slider = (int)Math.Round(ClientSize.Width * SliderSize);
            if (slider < 1)
                slider = 1;

            ArrayFrame frame = ArrayFrame.BuildFrame(ClientSize, Skin.GetBackgroundBlockID());
            frame.Overwrite(ArrayFrame.BuildFrame(slider, ClientSize.Height, Skin.GetForegroundBlockID()), new(position, 0));

            return frame;
        }
    }
}
