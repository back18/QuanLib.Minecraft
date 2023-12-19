using QuanLib.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Directorys
{
    public class ResourcePackDirectory : DirectoryBase
    {
        public ResourcePackDirectory(string directory) : base(directory)
        {
            BlockStates = Combine("blockstates");
            Models = AddDirectory<ModelsDirectory>("models");
            Textures = AddDirectory<TexturesDirectory>("textures");
        }

        public string BlockStates { get; }

        public ModelsDirectory Models { get; }

        public TexturesDirectory Textures { get; }

        public class ModelsDirectory : DirectoryBase
        {
            public ModelsDirectory(string directory) : base(directory)
            {
                BlockDir = AddDirectory<DirectoryBase>("block");
                ItemDir = AddDirectory<DirectoryBase>("item");
            }

            public DirectoryBase BlockDir { get; }

            public DirectoryBase ItemDir { get; }
        }

        public class TexturesDirectory : DirectoryBase
        {
            public TexturesDirectory(string directory) : base(directory)
            {
                BlockDir = AddDirectory<DirectoryBase>("block");
                ItemDir = AddDirectory<DirectoryBase>("item");
            }

            public DirectoryBase BlockDir { get; }

            public DirectoryBase ItemDir { get; }
        }
    }
}
