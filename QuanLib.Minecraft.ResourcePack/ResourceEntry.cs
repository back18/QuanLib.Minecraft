using QuanLib.IO.Zip;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.ResourcePack
{
    public class ResourceEntry
    {
        public ResourceEntry(string modID)
        {
            ArgumentException.ThrowIfNullOrEmpty(modID, nameof(modID));

            ModId = modID;
            Path = new(modID);
            BlockStates = new();
            BlockModels = new();
            BlockTextures = new();
            ItemModels = new();
            ItemTextures = new();
            Languages = new();
        }

        public string ModId { get; }

        public ResourcePath Path { get; }

        public Dictionary<string, ZipItem> BlockStates { get; }

        public Dictionary<string, ZipItem> BlockModels { get; }

        public Dictionary<string, ZipItem> BlockTextures { get; }

        public Dictionary<string, ZipItem> ItemModels { get; }

        public Dictionary<string, ZipItem> ItemTextures { get; }

        public Dictionary<string, ZipItem> Languages { get; }

        public bool IsEmpty => !BlockStates.Any() && !BlockModels.Any() && !BlockTextures.Any() && !ItemModels.Any() && !ItemTextures.Any();
    }
}
