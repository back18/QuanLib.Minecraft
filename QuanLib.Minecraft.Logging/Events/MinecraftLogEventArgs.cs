using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Logging.Events
{
    public class MinecraftLogEventArgs : EventArgs
    {
        public MinecraftLogEventArgs(MinecraftLog minecraftLog)
        {
            MinecraftLog = minecraftLog;
        }

        public MinecraftLog MinecraftLog { get; }
    }
}
