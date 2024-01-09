using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod.Fabric
{
    public struct MixinInfo
    {
        [JsonProperty("config")]
        public required string Config { get; set; }

        [JsonProperty("environment")]
        public required string Environment { get; set; }

        public override string ToString()
        {
            return Config;
        }
    }
}
