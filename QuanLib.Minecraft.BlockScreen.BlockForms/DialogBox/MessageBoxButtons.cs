using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms.DialogBox
{
    [Flags]
    public enum MessageBoxButtons
    {
        None = 0,

        Yes = 1,

        No = 2,

        OK = 4,

        Cancel = 8,

        Retry = 16
    }
}
