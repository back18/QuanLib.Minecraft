using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public interface IBytesCommandSender : ICommandSender
    {
        public byte[] CommandToBytes(string command);

        public void SendCommand(byte[] bytesCommand);

        public void SendAllCommand(IEnumerable<byte[]> bytesCommands);

        public Task SendCommandAsync(byte[] bytesCommand);

        public Task SendAllCommandAsync(IEnumerable<byte[]> bytesCommands);
    }
}
