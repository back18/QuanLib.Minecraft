using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.ResourcePack.Block.BlockTextureMaps
{
    public class CubeColumnTextureMap : TextureMap
    {
        public CubeColumnTextureMap(string blockState)
        {
            Xp = "side";
            Xm = "side";
            Yp = "side";
            Ym = "side";
            Zp = "side";
            Zm = "side";

            if (string.IsNullOrEmpty(blockState))
                blockState = "axis=y";

            if (!MinecraftUtil.TryParseBlockState(blockState, out var states))
                states = new();
            if (!states.TryGetValue("axis", out var axis))
                axis = "y";

            switch (axis)
            {
                case "x":
                    Xp = "end";
                    Xm = "end";
                    break;
                case "y":
                    Yp = "end";
                    Ym = "end";
                    break;
                case "z":
                    Zp = "end";
                    Zm = "end";
                    break;
                default:
                    goto case "y";
            }
        }

        public override string Name => "minecraft:block/cube_column";

        public override BlockType Type => BlockType.CubeColumn;

        public override string Xp { get; }

        public override string Xm { get; }

        public override string Yp { get; }

        public override string Ym { get; }

        public override string Zp { get; }

        public override string Zm { get; }
    }
}
