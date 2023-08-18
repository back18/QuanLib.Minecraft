using QuanLib.Minecraft.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public interface IPlane
    {
        public int Width { get; }

        public int Height { get; }

        public Facing NormalFacing { get; }
    }
}
