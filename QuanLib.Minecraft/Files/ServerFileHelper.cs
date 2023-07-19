using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Files
{
    public class ServerFileHelper : MinecraftFileHelper
    {
        public ServerFileHelper(ServerPathManager paths) : base(paths)
        {
            _paths = paths ?? throw new ArgumentNullException(nameof(paths));
        }

        private readonly ServerPathManager _paths;

        public Dictionary<string, string> GetServerProperties()
        {
            Dictionary<string, string> result = new();
            string[] lines = File.ReadAllLines(_paths.ServerPropertiesFile);
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
