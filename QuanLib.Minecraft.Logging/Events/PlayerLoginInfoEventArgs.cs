using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Logging.Events
{
    public class PlayerLoginInfoEventArgs : EventArgs
    {
        public PlayerLoginInfoEventArgs(PlayerLoginInfo playerLoginInfo)
        {
            PlayerLoginInfo = playerLoginInfo;
        }

        public PlayerLoginInfo PlayerLoginInfo { get; }
    }
}
