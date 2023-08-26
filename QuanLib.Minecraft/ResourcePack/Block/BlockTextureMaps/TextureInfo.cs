using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.ResourcePack.Block.BlockTextureMaps
{
    public class TextureInfo
    {
        public TextureInfo(BlockType type, string xp, string xm, string yp, string ym, string zp, string zm)
        {
            Type = type;
            Xp = xp;
            Xm = xm;
            Yp = yp;
            Ym = ym;
            Zp = zp;
            Zm = zm;
        }

        public BlockType Type { get; }

        public string Xp { get; }

        public string Xm { get; }

        public string Yp { get; }

        public string Ym { get; }

        public string Zp { get; }

        public string Zm { get; }
    }
}
