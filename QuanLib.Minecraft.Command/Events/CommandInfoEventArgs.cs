using QuanLib.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Events
{
    public readonly struct CommandInfoEventArgs(CommandInfo commandInfo) : IValueEventArgs
    {
        public readonly CommandInfo CommandInfo = commandInfo;
    }
}
