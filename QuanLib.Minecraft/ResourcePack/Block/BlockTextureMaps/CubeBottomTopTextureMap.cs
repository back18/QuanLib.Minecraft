using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.ResourcePack.Block.BlockTextureMaps
{
    public class CubeBottomTopTextureMap : TextureMap
    {
        public override string Name => "minecraft:block/cube_bottom_top";

        public override BlockType Type => BlockType.CubeBottomTop;

        public override string Xp => "side";

        public override string Xm => "side";

        public override string Yp => "top";

        public override string Ym => "bottom";

        public override string Zp => "side";

        public override string Zm => "side";
    }
}
