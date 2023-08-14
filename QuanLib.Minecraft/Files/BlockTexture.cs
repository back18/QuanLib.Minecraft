using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace QuanLib.Minecraft.Files
{
    public class BlockTexture
    {
        public BlockTexture(string blockID, BlockTextureType textureType, IReadOnlyDictionary<Facing, Image<Rgba32>> images)
        {
            if (string.IsNullOrEmpty(blockID))
                throw new ArgumentException($"“{nameof(blockID)}”不能为 null 或空。", nameof(blockID));

            BlockID = blockID;
            TextureType = textureType;
            Images = images ?? throw new ArgumentNullException(nameof(images));
            Dictionary<Facing, Rgba32> averageColors = new();
            foreach (var image in images)
                averageColors.Add(image.Key, GetAverageColors(image.Value));
            AverageColors = averageColors;
        }

        public string BlockID { get; }

        public BlockTextureType TextureType { get; }

        public IReadOnlyDictionary<Facing, Image<Rgba32>> Images { get; }

        public IReadOnlyDictionary<Facing, Rgba32> AverageColors { get; }

        private Rgba32 GetAverageColors(Image<Rgba32> image)
        {
            int r = 0, g = 0, b = 0, a = 0;
            for (int x = 0; x < image.Width; x++)
                for (int y = 0; y < image.Height; y++)
                {
                    r += image[x, y].R;
                    g += image[x, y].G;
                    b += image[x, y].B;
                    a += image[x, y].A;
                }
            int size = image.Width * image.Height;
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
