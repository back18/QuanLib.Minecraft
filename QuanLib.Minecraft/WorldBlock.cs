using QuanLib.Minecraft.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public readonly struct WorldBlock(BlockPos position, string blockID)
    {
        public BlockPos Position { get; } = position;

        public string BlockID { get; } = blockID;
    }
}
