using QuanLib.Minecraft;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public class ScreenPixel : Pixel
    {
        public ScreenPixel(Point position, string blockID) : base(blockID)
        {
            Position = position;
        }

        public Point Position { get; }
    }
}
