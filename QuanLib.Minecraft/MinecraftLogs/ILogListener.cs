using QuanLib.Core;
using QuanLib.IO;
using QuanLib.Minecraft.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.MinecraftLogs
{
    public interface ILogListener : ITextListener
    {
        public event EventHandler<ILogListener, MinecraftLogEventArgs> WriteLog;
    }
}
