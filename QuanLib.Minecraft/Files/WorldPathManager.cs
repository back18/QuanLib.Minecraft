using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Files
{
    public class WorldPathManager
    {
        public WorldPathManager(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException($"“{nameof(path)}”不能为 null 或空。", nameof(path));

            WorldDir = path;
            DataDir = Path.Combine(path, "data");
            PlayerDataDir = Path.Combine(path, "datapacks");
            ServerConfigDir = Path.Combine(path, "serverconfig");
            StatsDir = Path.Combine(path, "stats");
            LevelDatFile = Path.Combine(path, "level.dat");
        }

        public string WorldDir { get; }

        public string DataDir { get; }

        public string PlayerDataDir { get; }

        public string ServerConfigDir { get; }

        public string StatsDir { get; }

        public string LevelDatFile { get; }
    }
}
