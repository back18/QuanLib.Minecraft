using QuanLib.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Directorys
{
    public class MinecraftClientDirectory : MinecraftDirectory
    {
        public MinecraftClientDirectory(string directory) : base(directory)
        {
            SavesDir = AddDirectory<SavesDirectory>("saves");
        }

        public SavesDirectory SavesDir { get; }

        public override WorldDirectory? GetActiveWorldDirectory()
        {
            return SavesDir.GetActiveWorldDirectory();
        }
    }
}
