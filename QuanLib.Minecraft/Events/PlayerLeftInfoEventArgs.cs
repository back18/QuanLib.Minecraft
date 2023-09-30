using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Events
{
    public class PlayerLeftInfoEventArgs : EventArgs
    {
        public PlayerLeftInfoEventArgs(PlayerLeftInfo playerLeftInfo)
        {
            PlayerLeftInfo = playerLeftInfo;
        }

        public PlayerLeftInfo PlayerLeftInfo { get; }
    }
}
