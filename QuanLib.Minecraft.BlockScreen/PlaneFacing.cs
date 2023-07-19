using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    [Flags]
    public enum PlaneFacing
    {
        None = 0,

        Top = 1,

        Bottom = 2,

        Left = 4,

        Right = 8
    }
}
