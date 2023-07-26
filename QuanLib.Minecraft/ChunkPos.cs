using QuanLib.Minecraft.Vectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public struct SurfacePos
    {
        public SurfacePos(int x, int z)
        {
            X = x;
            Z = z;
        }

        public int X;

        public int Z;
    }
}
