using QuanLib.Minecraft.BlockScreen.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Event
{
    public class ScreenContextEventArgs : EventArgs
    {
        public ScreenContextEventArgs(ScreenContext screenContext)
        {
            ScreenContext = screenContext;
        }

        public ScreenContext ScreenContext { get; }
    }
}
