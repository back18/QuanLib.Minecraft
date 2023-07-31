using QuanLib.Minecraft.BlockScreen.Screens;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Event
{
    public class CursorEventArgs : EventArgs
    {
        public CursorEventArgs(Point position)
        {
            Position = position;
        }

        public Point Position { get; }
    }
}
