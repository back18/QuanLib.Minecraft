using QuanLib.IO.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.PathManagers
{
    public class ServerPathManager : MinecraftPathManager
    {
        public ServerPathManager(string minecraftDirectory) : base(minecraftDirectory)
        {
            _serverPaths = new(minecraftDirectory);
        }

        protected readonly ServerPaths _serverPaths;

        public DirectoryInfo Minecraft_World => _serverPaths.Minecraft_World.CreateDirectoryInfo();

        public FileInfo Minecraft_ServerProperties => _serverPaths.Minecraft_ServerProperties.CreateFileInfo();

        public FileInfo Minecraft_Whitelist => _serverPaths.Minecraft_Whitelist.CreateFileInfo();

        public FileInfo Minecraft_BannedPlayers => _serverPaths.Minecraft_BannedPlayers.CreateFileInfo();

        public FileInfo Minecraft_BannedIps => _serverPaths.Minecraft_BannedIps.CreateFileInfo();

        public FileInfo Minecraft_EulaFile => _serverPaths.Minecraft_EulaFile.CreateFileInfo();

        [DebuggerStepThrough]
        public override DirectoryInfo[] GetActiveWorlds()
        {
            WorldPathManager worldPathManager = new(Minecraft_World.FullName);

            try
            {
                using FileStream fileStream = worldPathManager.World_SessionLock.OpenRead();
                return [];
            }
            catch
            {
                return [worldPathManager.World];
            }
        }

        protected class ServerPaths : MinecraftPaths
        {
            public ServerPaths(string minecraftDirectory) : base(minecraftDirectory)
            {
                Minecraft_World = Minecraft.PathCombine("world");
                Minecraft_ServerProperties = Minecraft.PathCombine("server.properties");
                Minecraft_Whitelist = Minecraft.PathCombine("whitelist.json");
                Minecraft_BannedPlayers = Minecraft.PathCombine("banned-players.json");
                Minecraft_BannedIps = Minecraft.PathCombine("banned-ips.json");
                Minecraft_EulaFile = Minecraft.PathCombine("eula.txt");
            }

            public readonly string Minecraft_World;

            public readonly string Minecraft_ServerProperties;

            public readonly string Minecraft_Whitelist;

            public readonly string Minecraft_BannedPlayers;

            public readonly string Minecraft_BannedIps;

            public readonly string Minecraft_EulaFile;
        }
    }
}
