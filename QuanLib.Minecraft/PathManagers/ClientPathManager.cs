using QuanLib.IO.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.PathManagers
{
    public class ClientPathManager : MinecraftPathManager
    {
        public ClientPathManager(string minecraftDirectory) : base(minecraftDirectory)
        {
            _clientPaths = new(minecraftDirectory);
        }

        protected readonly ClientPaths _clientPaths;

        public DirectoryInfo Minecraft_Saves => _clientPaths.Minecraft_Saves.CreateDirectoryInfo();

        public DirectoryInfo Minecraft_ResourcePacks => _clientPaths.Minecraft_ResourcePacks.CreateDirectoryInfo();

        public DirectoryInfo Minecraft_ShaderPacks => _clientPaths.Minecraft_ShaderPacks.CreateDirectoryInfo();

        public DirectoryInfo Minecraft_Assets => _clientPaths.Minecraft_Assets.CreateDirectoryInfo();

        public DirectoryInfo Minecraft_Assets_Indexes => _clientPaths.Minecraft_Assets_Indexes.CreateDirectoryInfo();

        public DirectoryInfo Minecraft_Assets_Objects => _clientPaths.Minecraft_Assets_Objects.CreateDirectoryInfo();

        public DirectoryInfo Minecraft_Assets_Skins => _clientPaths.Minecraft_Assets_Skins.CreateDirectoryInfo();

        public DirectoryInfo Minecraft_DefaultConfigs => _clientPaths.Minecraft_DefaultConfigs.CreateDirectoryInfo();

        public FileInfo Minecraft_Options => _clientPaths.Minecraft_Options.CreateFileInfo();

        public override DirectoryInfo[] GetActiveWorlds()
        {
            List<DirectoryInfo> result = [];
            foreach (DirectoryInfo worldDirectory in Minecraft_Saves.GetDirectories())
            {
                WorldPathManager worldPathManager = new(worldDirectory.FullName);

                try
                {
                    using FileStream fileStream = worldPathManager.World_SessionLock.OpenRead();
                }
                catch
                {
                    result.Add(worldDirectory);
                }
            }
            return result.ToArray();
        }

        protected class ClientPaths : MinecraftPaths
        {
            public ClientPaths(string minecraftDirectory) : base(minecraftDirectory)
            {
                Minecraft_Saves = Minecraft.PathCombine("saves");
                Minecraft_ResourcePacks = Minecraft.PathCombine("resourcepacks");
                Minecraft_ShaderPacks = Minecraft.PathCombine("shaderpacks");
                Minecraft_Assets = Minecraft.PathCombine("assets");
                Minecraft_Assets_Indexes = Minecraft_Assets.PathCombine("indexes");
                Minecraft_Assets_Objects = Minecraft_Assets.PathCombine("objects");
                Minecraft_Assets_Skins = Minecraft_Assets.PathCombine("skins");
                Minecraft_DefaultConfigs = Minecraft.PathCombine("defaultconfigs");
                Minecraft_Options = Minecraft.PathCombine("options.txt");
            }

            public readonly string Minecraft_Saves;

            public readonly string Minecraft_ResourcePacks;

            public readonly string Minecraft_ShaderPacks;

            public readonly string Minecraft_Assets;

            public readonly string Minecraft_Assets_Indexes;

            public readonly string Minecraft_Assets_Objects;

            public readonly string Minecraft_Assets_Skins;

            public readonly string Minecraft_DefaultConfigs;

            public readonly string Minecraft_Options;
        }
    }
}
