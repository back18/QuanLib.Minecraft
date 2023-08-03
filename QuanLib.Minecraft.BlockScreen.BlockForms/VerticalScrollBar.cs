using QuanLib.Minecraft.BlockScreen.Frame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public class VerticalScrollBar : ScrollBar
    {
        public override IFrame RenderingFrame()
        {
            int slider = (int)Math.Round(ClientSize.Height * SliderSize);
            int position = (int)Math.Round(ClientSize.Height * SliderPosition);

            ArrayFrame frame = ArrayFrame.BuildFrame(ClientSize.Width, ClientSize.Height, Skin.GetBackgroundBlockID());
            frame.Overwrite(ArrayFrame.BuildFrame(ClientSize.Width, slider, Skin.GetForegroundBlockID()), new(0, position));

            return frame;
        }
    }
}
