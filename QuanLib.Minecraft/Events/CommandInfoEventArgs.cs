using QuanLib.Minecraft.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Events
{
    public class CommandInfoEventArgs : EventArgs
    {
        public CommandInfoEventArgs(CommandInfo commandInfo)
        {
            CommandInfo = commandInfo;
        }

        public CommandInfo CommandInfo { get; }
    }
}
