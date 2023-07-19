using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Files
{
    public class BlockTexture
    {
        public BlockTexture(string blockID, BlockTextureType textureType, IReadOnlyDictionary<Facing, Image<Rgba32>> images, IReadOnlyDictionary<Facing, Rgba32> averageColors)
        {
            if (string.IsNullOrEmpty(blockID))
                throw new ArgumentException($"“{nameof(blockID)}”不能为 null 或空。", nameof(blockID));

            BlockID = blockID;
            Images = images ?? throw new ArgumentNullException(nameof(images));
            AverageColors = averageColors ?? throw new ArgumentNullException(nameof(averageColors));
            TextureType = textureType;
        }

        public string BlockID { get; }

        public BlockTextureType TextureType { get; }

        public IReadOnlyDictionary<Facing, Image<Rgba32>> Images { get; }

        public IReadOnlyDictionary<Facing, Rgba32> AverageColors { get; }

        public override string ToString()
        {
            return BlockID;
        }
    }
}
