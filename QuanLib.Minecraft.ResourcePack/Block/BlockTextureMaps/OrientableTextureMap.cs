using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.ResourcePack.Block.BlockTextureMaps
{
    public class OrientableTextureMap : TextureMap
    {
        public OrientableTextureMap(string blockState)
        {
            Xp = "side";
            Xm = "side";
            Yp = "top";
            Ym = "top";
            Zp = "side";
            Zm = "side";

            if (string.IsNullOrEmpty(blockState))
                blockState = "facing=north";

            if (!MinecraftUtil.TryParseBlockState(blockState, out var states))
                states = new();
            if (!states.TryGetValue("facing", out var facing))
                facing = "north";

            switch (facing)
            {
                case "east":
                    Xp = "front";
                    break;
                case "west":
                    Xm = "front";
                    break;
                case "south":
                    Zp = "front";
                    break;
                case "north":
                    Zm = "front";
                    break;
                default:
                    goto case "north";
            }
        }

        public override string Name => "minecraft:block/orientable";

        public override BlockType Type => BlockType.Orientable;

        public override string Xp { get; }

        public override string Xm { get; }

        public override string Yp { get; }

        public override string Ym { get; }

        public override string Zp { get; }

        public override string Zm { get; }
    }
}
