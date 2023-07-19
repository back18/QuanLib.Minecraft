using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Files
{
    public interface ILogListener
    {
        public event Action<MinecraftLog> OnWriteLog;
    }
}
