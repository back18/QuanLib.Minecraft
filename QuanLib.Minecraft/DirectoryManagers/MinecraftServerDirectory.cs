using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.DirectoryManagers
{
    public class MinecraftServerDirectory : MinecraftDirectory
    {
        public MinecraftServerDirectory(string directory) : base(directory)
        {
            World = new(Combine("world"));
            BannedIps = new(Combine("banned-ips.json"));
            BannedPlayers = new(Combine("banned-players.json"));
            Eula = new(Combine("eula.txt"));
            Ops = new(Combine("ops.json"));
            ServerProperties = new(Combine("server.properties"));
            Whitelist = new(Combine("whitelist.json"));
        }

        public WorldDirectory World { get; }

        public string BannedIps { get; }

        public string BannedPlayers { get; }

        public string Eula { get; }

        public string Ops { get; }

        public string ServerProperties { get; }

        public string Whitelist { get; }

        public Dictionary<string, string> ReadServerProperties()
        {
            Dictionary<string, string> result = new();
            string[] lines = File.ReadAllLines(ServerProperties);
            foreach (string line in lines)
            {
                if (line.StartsWith('#'))
                    continue;
                string[] kv = line.Split('=');
                if (kv.Length < 2)
                    continue;
                result.Add(kv[0], kv[1]);
            }
            return result;
        }
    }
}
