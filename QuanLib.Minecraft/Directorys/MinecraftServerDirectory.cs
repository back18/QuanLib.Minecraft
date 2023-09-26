using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Directorys
{
    public class MinecraftServerDirectory : MinecraftDirectory
    {
        public MinecraftServerDirectory(string directory) : base(directory)
        {
            WorldDir = AddDirectory<WorldDirectory>("world");
            BannedIpsFile = Combine("banned-ips.json");
            BannedPlayersFile = Combine("banned-players.json");
            EulaFile = Combine("eula.txt");
            OpsFile = Combine("ops.json");
            ServerPropertiesFile = Combine("server.properties");
            WhitelistFile = Combine("whitelist.json");
        }

        public WorldDirectory WorldDir { get; }

        public string BannedIpsFile { get; }

        public string BannedPlayersFile { get; }

        public string EulaFile { get; }

        public string OpsFile { get; }

        public string ServerPropertiesFile { get; }

        public string WhitelistFile { get; }

        public override WorldDirectory? GetActiveWorldDirectory()
        {
            if (WorldDir.IsLocked())
                return WorldDir;
            else
                return null;
        }
    }
}
