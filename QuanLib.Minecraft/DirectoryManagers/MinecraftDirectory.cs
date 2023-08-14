using QuanLib.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.DirectoryManagers
{
    public abstract class MinecraftDirectory : DirectoryManager
    {
        public MinecraftDirectory(string directory) : base(directory)
        {
            Config = new(Combine("config"));
            CrashReports = new(Combine("crash-reports"));
            Libraries = new(Combine("libraries"));
            Logs = new(Combine("logs"));
            Mods = new(Combine("mods"));
            Versions = new(Combine("versions"));
        }

        public ConfigDirectory Config { get; }

        public CrashReportsDirectory CrashReports { get; }

        public LibrariesDirectory Libraries { get; }

        public LogsDirectory Logs { get; }

        public ModsDirectory Mods { get; }

        public VersionsDirectory Versions { get; }
    }
}
