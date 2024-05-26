using QuanLib.IO.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.PathManagers
{
    public class MinecraftPathManager
    {
        public MinecraftPathManager(string minecraftDirectory)
        {
            ArgumentException.ThrowIfNullOrEmpty(minecraftDirectory, nameof(minecraftDirectory));

            _minecraftPaths = new(minecraftDirectory);
        }

        protected readonly MinecraftPaths _minecraftPaths;

        public DirectoryInfo Minecraft => _minecraftPaths.Minecraft.CreateDirectoryInfo();

        public DirectoryInfo Minecraft_Mods => _minecraftPaths.Minecraft_Mods.CreateDirectoryInfo();

        public DirectoryInfo Minecraft_Config => _minecraftPaths.Minecraft_Config.CreateDirectoryInfo();

        public DirectoryInfo Minecraft_Logs => _minecraftPaths.Minecraft_Logs.CreateDirectoryInfo();

        public FileInfo Minecraft_Logs_LatestLog => _minecraftPaths.Minecraft_Logs_LatestLog.CreateFileInfo();

        public DirectoryInfo Minecraft_Versions => _minecraftPaths.Minecraft_Versions.CreateDirectoryInfo();

        public DirectoryInfo Minecraft_Libraries => _minecraftPaths.Minecraft_Libraries.CreateDirectoryInfo();

        public DirectoryInfo Minecraft_CrashReports => _minecraftPaths.Minecraft_CrashReports.CreateDirectoryInfo();

        public virtual DirectoryInfo[] GetActiveWorlds()
        {
            return [];
        }

        protected class MinecraftPaths
        {
            public MinecraftPaths(string minecraftDirectory)
            {
                ArgumentException.ThrowIfNullOrEmpty(minecraftDirectory, nameof(minecraftDirectory));

                Minecraft = minecraftDirectory;
                Minecraft_Mods = Minecraft.PathCombine("mods");
                Minecraft_Config = Minecraft.PathCombine("config");
                Minecraft_Logs = Minecraft.PathCombine("logs");
                Minecraft_Logs_LatestLog = Minecraft_Logs.PathCombine("latest.log");
                Minecraft_Versions = Minecraft.PathCombine("versions");
                Minecraft_Libraries = Minecraft.PathCombine("libraries");
                Minecraft_CrashReports = Minecraft.PathCombine("crash-reports");
            }

            public readonly string Minecraft;

            public readonly string Minecraft_Mods;

            public readonly string Minecraft_Config;

            public readonly string Minecraft_Logs;

            public readonly string Minecraft_Logs_LatestLog;

            public readonly string Minecraft_Versions;

            public readonly string Minecraft_Libraries;

            public readonly string Minecraft_CrashReports;
        }
    }
}
