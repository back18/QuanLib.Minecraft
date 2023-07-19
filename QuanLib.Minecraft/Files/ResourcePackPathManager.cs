using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Files
{
    public class ResourcePackPathManager
    {
        public ResourcePackPathManager(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException($"“{nameof(path)}”不能为 null 或空。", nameof(path));

            MainDir = path;
            BlockStatesDir = Path.Combine(MainDir, "blockstates");
            BlockModelsDir = Path.Combine(MainDir, "models", "block");
            ItemModelsDir = Path.Combine(MainDir, "models", "item");
            BlockTexturesDir = Path.Combine(MainDir, "textures", "block");
            ItemTexturesDir = Path.Combine(MainDir, "textures", "item");
        }

        public string MainDir { get; }

        public string BlockStatesDir { get; }

        public string BlockModelsDir { get; }

        public string ItemModelsDir { get; }

        public string BlockTexturesDir { get; }

        public string ItemTexturesDir { get; }
    }
}
