using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLib.Minecraft.Downloading
{
    public class VersionJsonPatch(JObject jobj) : VersionJsonBase(jobj)
    {
        public string Version => JObject["version"]?.Value<string>() ?? string.Empty;

        public string InheritsFrom => JObject["inheritsFrom"]?.Value<string>() ?? string.Empty;

        public int Priority => JObject["priority"]?.Value<int>() ?? 0;
    }
}
