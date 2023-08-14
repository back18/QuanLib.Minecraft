using QuanLib.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.DirectoryManagers
{
    public class ResourcePackDirectory : DirectoryManager
    {
        public ResourcePackDirectory(string directory) : base(directory)
        {
            BlockStates = Combine("blockstates");
            Models = new(Combine("models"));
            Textures = new(Combine("textures"));
        }

        public string BlockStates { get; }

        public ModelsDirectory Models { get; }

        public TexturesDirectory Textures { get; }

        public class ModelsDirectory : DirectoryManager
        {
            public ModelsDirectory(string directory) : base(directory)
            {
                Block = new(Combine("block"));
                Item = new(Combine("item"));
            }

            public string Block { get; }

            public string Item { get; }
        }

        public class TexturesDirectory : DirectoryManager
        {
            public TexturesDirectory(string directory) : base(directory)
            {
                Block = new(Combine("block"));
                Item = new(Combine("item"));
            }

            public string Block { get; }

            public string Item { get; }
        }
    }
}
