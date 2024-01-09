using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod.Fabric
{
    public struct JarInfo
    {
        [JsonProperty("file")]
        public required string File { get; set; }

        public override string ToString()
        {
            return File;
        }
    }
}
