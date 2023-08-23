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
            Minecraft = Combine("Minecraft.toml");
            System = Combine("System.toml");
            Screen = Combine("Screen.toml");
            Registry = Combine("Registry.json");
            Log4Net = Combine("log4net.xml");
        }

        public string Minecraft { get; }

        public string System { get; }

        public string Screen { get; }

        public string Registry { get; }

        public string Log4Net { get; }
    }
}
