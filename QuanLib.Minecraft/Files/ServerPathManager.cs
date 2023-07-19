using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Files
{
    public class ServerPathManager : MinecraftPathManager
    {
        public ServerPathManager(string path) : base(path)
        {
            BannedIpsFile = Path.Combine(path, "banned-ips.json");
            BannedPlayersFile = Path.Combine(path, "banned-players.json");
            OpsFile = Path.Combine(path, "ops.json");
            ServerPropertiesFile = Path.Combine(path, "server.properties");
            WhiteListFile = Path.Combine(path, "whitelist.json");
        }

        public string BannedIpsFile { get; }

        public string BannedPlayersFile { get; }

        public string OpsFile { get; }

        public string ServerPropertiesFile { get; }

        public string WhiteListFile { get; }
    }
}
