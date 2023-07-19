using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BuiltInApps.DialogBox
{
    [Flags]
    public enum DialogBoxReturnValue
    {
        None = 0,

        Yes = 1,

        No = 2,

        OK = 4,

        Cancel = 8
    }
}
