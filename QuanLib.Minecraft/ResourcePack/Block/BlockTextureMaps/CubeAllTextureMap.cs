using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.ResourcePack.Block.BlockTextureMaps
{
    public class CubeAllTextureMap : TextureMap
    {
        public override string Name => "minecraft:block/cube_all";

        public override BlockType Type => BlockType.CubeAll;

        public override string Xp => "all";

        public override string Xm => "all";

        public override string Yp => "all";

        public override string Ym => "all";

        public override string Zp => "all";

        public override string Zm => "all";
    }
}
