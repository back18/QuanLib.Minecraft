using CoreRCON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Instance
{
    public interface IRconCapable
    {
        public ushort RconPort { get; }

        public string RconPassword { get; }

        public RCON RCON { get; }
    }
}
