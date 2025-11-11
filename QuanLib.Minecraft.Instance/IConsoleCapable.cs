using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Instance
{
    public interface IConsoleCapable
    {
        public ServerProcess ServerProcess { get; }

        public ServerConsole ServerConsole { get; }
    }
}
