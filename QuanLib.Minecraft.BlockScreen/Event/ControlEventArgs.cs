using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Event
{
    public class ControlEventArgs<TControl> : EventArgs where TControl : class, IControl
    {
        public ControlEventArgs(TControl control)
        {
            Control = control;
        }

        public TControl Control { get; }
    }
}
