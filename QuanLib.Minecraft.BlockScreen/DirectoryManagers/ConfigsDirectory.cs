using QuanLib.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.DirectoryManagers
{
    public class ConfigsDirectory : DirectoryManager
    {
        public ConfigsDirectory(string directory) : base(directory)
        {
            Minecraft = Combine("Minecraft.json");
            System = Combine("System.json");
            Screen = Combine("Screen.json");
            Registry = Combine("Registry.json");
        }

        public string Minecraft { get; }

        public string System { get; }

        public string Screen { get; }

        public string Registry { get; }
    }
}
