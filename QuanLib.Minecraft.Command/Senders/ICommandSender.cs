using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Senders
{
    public interface ICommandSender
    {
        public TimeSpan Ping();

        public Task<TimeSpan> PingAsync();
    }
}
