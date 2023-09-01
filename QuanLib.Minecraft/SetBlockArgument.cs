using QuanLib.Minecraft.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public readonly struct SetBlockArgument : ISetBlockArgument
    {
        public SetBlockArgument(BlockPos position, string blockID)
        {
            Position = position;
            BlockID = blockID;
        }

        public BlockPos Position { get; }

        public string BlockID { get; }
    }
}
