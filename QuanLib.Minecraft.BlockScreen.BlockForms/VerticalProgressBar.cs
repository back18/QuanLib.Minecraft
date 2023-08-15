using QuanLib.Minecraft.BlockScreen.Frame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public class VerticalProgressBar : ProgressBar
    {
        public override IFrame RenderingFrame()
        {
            int length = (int)Math.Round(ClientSize.Height * Progress);
            if (length < 0)
                length = 0;

            ArrayFrame frame = ArrayFrame.BuildFrame(ClientSize, Skin.GetBackgroundBlockID());
            frame.Overwrite(ArrayFrame.BuildFrame(ClientSize.Width, length, Skin.GetForegroundBlockID()), new(0, 0));
            return frame;
        }
    }
}
