using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Senders
{
    public interface IOnewayCommandSender : ICommandSender
    {
        public void SendOnewayCommand(string command);

        public Task SendOnewayCommandAsync(string command);

        public void SendOnewayBatchCommand(IEnumerable<string> commands);

        public Task SendOnewayBatchCommandAsync(IEnumerable<string> commands);

        public void SendOnewayBatchSetBlock(IEnumerable<ISetBlockArgument> arguments);

        public Task SendOnewayBatchSetBlockAsync(IEnumerable<ISetBlockArgument> arguments);
    }
}
