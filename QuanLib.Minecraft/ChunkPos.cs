using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public struct ChunkPos(int x, int z)
    {
        public int X { get; set; } = x;

        public int Z { get; set; } = z;
    }
}
