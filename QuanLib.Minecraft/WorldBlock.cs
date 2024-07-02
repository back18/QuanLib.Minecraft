using QuanLib.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public readonly struct WorldBlock(Vector3<int> position, string blockId)
    {
        public readonly Vector3<int> Position = position;

        public readonly string BlockId = blockId;
    }
}
