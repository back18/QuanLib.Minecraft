using QuanLib.Minecraft.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public interface ISetBlockArgument
    {
        public BlockPos Position { get; }

        public string BlockID { get; }
    }
}
