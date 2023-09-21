using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Sender
{
    public interface ICommandSender
    {
        public void WaitForResponse();

        public Task WaitForResponseAsync();
    }
}
