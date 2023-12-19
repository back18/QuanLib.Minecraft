using QuanLib.Core.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Directorys
{
    public class MinecraftDirectory : DirectoryBase
    {
        public MinecraftDirectory(string directory) : base(directory)
        {
            ConfigDir = AddDirectory<ConfigDirectory>("config");
            CrashReportsDir = AddDirectory<CrashReportsDirectory>("crash-reports");
            LibrariesDir = AddDirectory<LibrariesDirectory>("libraries");
            LogsDir = AddDirectory<LogsDirectory>("logs");
            ModsDir = AddDirectory<ModsDirectory>("mods");
            VersionsDir = AddDirectory<VersionsDirectory>("versions");
        }

        public ConfigDirectory ConfigDir { get; }

        public CrashReportsDirectory CrashReportsDir { get; }

        public LibrariesDirectory LibrariesDir { get; }

        public LogsDirectory LogsDir { get; }

        public ModsDirectory ModsDir { get; }

        public VersionsDirectory VersionsDir { get; }

        public virtual WorldDirectory? GetActiveWorldDirectory()
        {
            return null;
        }
    }
}
