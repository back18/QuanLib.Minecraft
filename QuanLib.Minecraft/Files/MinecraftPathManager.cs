using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Files
{
    public class MinecraftPathManager
    {
        public MinecraftPathManager(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException($"“{nameof(path)}”不能为 null 或空。", nameof(path));

            MainDir = path;
            ConfigDir = Path.Combine(path, "config");
            DefaultConfigsDir = Path.Combine(path, "defaultconfigs");
            LibrariesDir = Path.Combine(path, "libraries");
            LogsDir = Path.Combine(path, "logs");
            LatestLogFile = Path.Combine(LogsDir, "latest.log");
            ModsDir = Path.Combine(path, "mods");
        }

        public string MainDir { get; }

        public string ConfigDir { get; }

        public string DefaultConfigsDir { get; }

        public string LibrariesDir { get; }

        public string LogsDir { get; }

        public string LatestLogFile { get; }

        public string ModsDir { get; }
    }
}
