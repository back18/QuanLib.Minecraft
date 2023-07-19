using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Controls
{
    [Flags]
    public enum TitleBarButtonType
    {
        None = 0,

        Close = 1,

        MaximizeOrRestore = 2,

        Minimize = 4,

        FullScreen = 8
    }
}
