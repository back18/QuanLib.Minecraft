using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace QuanLib.Minecraft.ResourcePack.Block
{
    public class BlockTexture
    {
        public BlockTexture(string blockID, BlockType blockType, IReadOnlyDictionary<Facing, Image<Rgba32>> textures)
        {
            if (string.IsNullOrEmpty(blockID))
                throw new ArgumentException($"“{nameof(blockID)}”不能为 null 或空。", nameof(blockID));

            BlockID = blockID;
            BlockType = blockType;
            Textures = textures ?? throw new ArgumentNullException(nameof(textures));
            Dictionary<Facing, Rgba32> averageColors = new();
            foreach (var image in textures)
                averageColors.Add(image.Key, GetAverageColors(image.Value));
            AverageColors = averageColors;
        }

        public string BlockID { get; }

        public BlockType BlockType { get; }

        public IReadOnlyDictionary<Facing, Image<Rgba32>> Textures { get; }

        public IReadOnlyDictionary<Facing, Rgba32> AverageColors { get; }

        private static Rgba32 GetAverageColors(Image<Rgba32> texture)
        {
            int r = 0, g = 0, b = 0, a = 0;
            for (int x = 0; x < texture.Width; x++)
                for (int y = 0; y < texture.Height; y++)
                {
                    r += texture[x, y].R;
                    g += texture[x, y].G;
                    b += texture[x, y].B;
                    a += texture[x, y].A;
                }
            int size = texture.Width * texture.Height;
            r /= size;
            g /= size;
            b /= size;
            a /= size;
            return new((byte)r, (byte)g, (byte)b, (byte)a);
        }

        public override string ToString()
        {
            return BlockID;
        }
    }
}
