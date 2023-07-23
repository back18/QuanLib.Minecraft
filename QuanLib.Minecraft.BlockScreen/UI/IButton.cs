using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.UI
{
    public interface IButton
    {
        public int ReboundTime { get; set; }

        public int ReboundCountdown { get; }
    }
}
