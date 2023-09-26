using QuanLib.Core.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Directorys
{
    public class ResourcePackDirectory : DirectoryManager
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

        public class ModelsDirectory : DirectoryManager
        {
            public ModelsDirectory(string directory) : base(directory)
            {
                BlockDir = AddDirectory<DirectoryManager>("block");
                ItemDir = AddDirectory<DirectoryManager>("item");
            }

            public DirectoryManager BlockDir { get; }

            public DirectoryManager ItemDir { get; }
        }

        public class TexturesDirectory : DirectoryManager
        {
            public TexturesDirectory(string directory) : base(directory)
            {
                BlockDir = AddDirectory<DirectoryManager>("block");
                ItemDir = AddDirectory<DirectoryManager>("item");
            }

            public DirectoryManager BlockDir { get; }

            public DirectoryManager ItemDir { get; }
        }
    }
}
