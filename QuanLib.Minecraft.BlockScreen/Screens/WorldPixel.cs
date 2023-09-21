using QuanLib.Minecraft.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Screens
{
    public class WorldPixel : Pixel, ISetBlockArgument
    {
        public WorldPixel(BlockPos position, string blockID) : base(blockID)
        {
            Position = position;
        }

        public BlockPos Position { get; }
    }
}
