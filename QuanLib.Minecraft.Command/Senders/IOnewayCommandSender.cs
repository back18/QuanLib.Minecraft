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

        public void SendOnewayBatchCommand(IList<string> commands);

        public Task SendOnewayBatchCommandAsync(IList<string> commands);

        public Task SendOnewayDelayBatchCommandAsync(IList<string> commands, Task? delay);

        public void SendOnewayBatchSetBlock(IList<WorldBlock> arguments);

        public Task SendOnewayBatchSetBlockAsync(IList<WorldBlock> arguments);

        public Task SendOnewayDelayBatchSetBlockAsync(IList<WorldBlock> arguments, Task? delay);
    }
}
