using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Screens
{
    public abstract class Pixel
    {
        protected Pixel(string blockID)
        {
            BlockID = blockID ?? throw new ArgumentNullException(nameof(blockID));
        }

        public string BlockID { get; }
    }
}
