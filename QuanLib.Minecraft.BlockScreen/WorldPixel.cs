using QuanLib.Minecraft;
using QuanLib.Minecraft.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public class WorldPixel : Pixel
    {
        public WorldPixel(Vector3<int> position, string blockID) : base(blockID)
        {
            Position = position;
        }

        public Vector3<int> Position { get; }

        public string ToSetBlock()
        {
            return $"setblock {Position.X} {Position.Y} {Position.Z} {BlockID}";
        }
    }
}
