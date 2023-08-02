using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public enum MinecraftServerMode
    {
        /// <summary>
        /// RCON连接模式
        /// </summary>
        RconConnect,

        /// <summary>
        /// 托管进程模式
        /// </summary>
        ManagedProcess,
    }
}
