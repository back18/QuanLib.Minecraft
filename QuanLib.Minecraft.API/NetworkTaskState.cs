using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API
{
    public enum NetworkTaskState
    {
        NotStarted,

        WaitForReceive,

        Failed,

        Timeout,

        Completed
    }
}
