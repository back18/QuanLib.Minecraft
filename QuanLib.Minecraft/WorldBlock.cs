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
        public Vector3<int> Position { get; } = position;

        public string BlockId { get; } = blockId;
    }
}
