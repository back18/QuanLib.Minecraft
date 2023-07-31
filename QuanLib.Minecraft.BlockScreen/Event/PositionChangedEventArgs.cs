using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Event
{
    public class PositionChangedEventArgs : EventArgs
    {
        public PositionChangedEventArgs(Point oldPosition, Point newPosition)
        {
            OldPosition = oldPosition;
            NewPosition = newPosition;
        }

        public Point OldPosition { get; }

        public Point NewPosition { get; }
    }
}
