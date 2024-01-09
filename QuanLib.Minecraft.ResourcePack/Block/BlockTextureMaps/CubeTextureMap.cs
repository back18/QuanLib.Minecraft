using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.ResourcePack.Block.BlockTextureMaps
{
    public class CubeTextureMap : TextureMap
    {
        public override string Name => "minecraft:block/cube";

        public override BlockType Type => BlockType.Cube;

        public override string Xp => "east";

        public override string Xm => "west";

        public override string Yp => "up";

        public override string Ym => "down";

        public override string Zp => "south";

        public override string Zm => "north";
    }
}
