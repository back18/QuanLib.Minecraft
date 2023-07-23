using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.UI
{
    public interface ISkin
    {
        public string GetForegroundBlockID();

        public string GetBackgroundBlockID();

        public string GetBorderBlockID();

        public ImageFrame? GetBackgroundImage();

        public void SetAllForegroundBlockID(string blockID);

        public void SetAllBackgroundBlockID(string blockID);

        public void SetAllBorderBlockID(string blockID);

        public void SetAllBackgroundImage(ImageFrame? frame);
    }
}
