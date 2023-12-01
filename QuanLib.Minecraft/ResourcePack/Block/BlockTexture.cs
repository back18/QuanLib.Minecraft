using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace QuanLib.Minecraft.ResourcePack.Block
{
    public class BlockTexture
    {
        public BlockTexture(string blockID, BlockType blockType, IDictionary<Facing, Image<Rgba32>> images)
        {
            ArgumentException.ThrowIfNullOrEmpty(blockID, nameof(blockID));
            ArgumentNullException.ThrowIfNull(images, nameof(images));

            BlockID = blockID;
            BlockType = blockType;
            Textures = new(images.ToDictionary(item => item.Key, item => new Texture(item.Value)));
        }

        public string BlockID { get; }

        public BlockType BlockType { get; }

        public ReadOnlyDictionary<Facing, Texture> Textures { get; }

        public override string ToString()
        {
            return BlockID;
        }
    }
}
