using QuanLib.Minecraft.Data;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Event
{
    public class CursorItemEventArgs : CursorEventArgs
    {
        public CursorItemEventArgs(Point position, Item? item) : base(position)
        {
            Item = item;
        }

        public Item? Item { get; }
    }
}
