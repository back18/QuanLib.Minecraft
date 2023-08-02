using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Event
{
    public class CursorSlotEventArgs : CursorEventArgs
    {
        public CursorSlotEventArgs(Point position, int oldSlot, int newSlot) : base(position)
        {
            OldSlot = oldSlot;
            NewSlot = newSlot;
        }

        public int OldSlot { get; }

        public int NewSlot { get; }

        public int Delta
        {
            get
            {
                int delta = NewSlot - OldSlot;
                if (delta >= 6)
                    delta -= 9;
                else if (delta <= -6)
                    delta += 9;
                return delta;
            }
        }
    }
}
