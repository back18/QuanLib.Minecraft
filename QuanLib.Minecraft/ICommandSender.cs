using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public interface ICommandSender
    {
        public void SendCommand(string command);

        public void SendAllCommand(IEnumerable<string> commands);

        public Task SendCommandAsync(string command);

        public Task SendAllCommandAsync(IEnumerable<string> commands);
    }
}
