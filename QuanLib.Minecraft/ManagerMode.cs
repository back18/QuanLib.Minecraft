using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public enum ServerManagerMode
    {
        /// <summary>
        /// 未连接/无连接
        /// </summary>
        NotConnected,

        /// <summary>
        /// 托管进程模式
        /// </summary>
        ManagedProcess,

        /// <summary>
        /// 监听器模式
        /// </summary>
        Listener
    }
}
