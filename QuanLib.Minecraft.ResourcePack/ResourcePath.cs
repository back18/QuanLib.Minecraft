using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.ResourcePack
{
    public class ResourcePath
    {
        public ResourcePath(string modID)
        {
            ArgumentException.ThrowIfNullOrEmpty(modID, nameof(modID));

            BlockStates = $"assets/{modID}/blockstates/";
            BlockModels = $"assets/{modID}/models/block/";
            BlockTextures = $"assets/{modID}/textures/block/";
            ItemModels = $"assets/{modID}/models/item/";
            ItemTextures = $"assets/{modID}/textures/item/";
            Languages = $"assets/{modID}/lang/";
        }

        public string BlockStates { get; }

        public string BlockModels { get; }

        public string BlockTextures { get; }

        public string ItemModels { get; }

        public string ItemTextures { get; }

        public string Languages { get; }
    }
}
