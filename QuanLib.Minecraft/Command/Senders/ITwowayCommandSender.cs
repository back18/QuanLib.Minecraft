using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Senders
{
    public interface ITwowayCommandSender : ICommandSender
    {
        public string SendCommand(string command);

        public Task<string> SendCommandAsync(string command);

        public string[] SendBatchCommand(IEnumerable<string> commands);

        public Task<string[]> SendBatchCommandAsync(IEnumerable<string> commands);
    }
}
