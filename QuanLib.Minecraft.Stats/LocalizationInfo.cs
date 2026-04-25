using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLib.Minecraft.Stats
{
    public record class LocalizationInfo([JsonProperty("name")] string Name, [JsonProperty("description")] string Description);
}
