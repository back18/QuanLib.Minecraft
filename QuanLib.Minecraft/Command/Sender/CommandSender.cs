using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Sender
{
    public class CommandSender
    {
        public CommandSender(ITwowayCommandSender twowaySender, IOnewayCommandSender onewaySender)
        {
            TwowaySender = twowaySender ?? throw new ArgumentNullException(nameof(twowaySender));
            OnewaySender = onewaySender ?? throw new ArgumentNullException(nameof(onewaySender));
        }

        public ITwowayCommandSender TwowaySender { get; }

        public IOnewayCommandSender OnewaySender { get; }

        public string SendCommand(string command)
        {
            return TwowaySender.SendCommand(command);
        }

        public async Task<string> SendCommandAsync(string command)
        {
            return await TwowaySender.SendCommandAsync(command);
        }
    }
}
