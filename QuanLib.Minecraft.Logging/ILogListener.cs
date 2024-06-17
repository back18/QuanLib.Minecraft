using QuanLib.Core;
using QuanLib.Core.Events;
using QuanLib.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Logging
{
    public interface ILogListener : ITextListener
    {
        public event EventHandler<ILogListener, EventArgs<MinecraftLog>> WriteLog;
    }
}
