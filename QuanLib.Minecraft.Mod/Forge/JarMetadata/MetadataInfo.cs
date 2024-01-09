using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod.Forge.JarMetadata
{
    public struct MetadataInfo
    {
        [JsonProperty("path")]
        public required string Path { get; set; }

        [JsonProperty("identifier")]
        public required Identifier Identifier { get; set; }

        [JsonProperty("version")]
        public required Version Version { get; set; }

        [JsonProperty("isObfuscated")]
        public required bool IsObfuscated { get; set; }
    }
}
