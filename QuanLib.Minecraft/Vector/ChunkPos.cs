using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Vector
{
    public struct ChunkPos
    {
        public ChunkPos(int x, int z)
        {
            X = x;
            Z = z;
        }

        public int X { get; set; }

        public int Z { get; set; }
    }
}
