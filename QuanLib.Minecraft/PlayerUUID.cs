using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public class PlayerUUID
    {
        public PlayerUUID(string name, string uuid)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException($"“{nameof(name)}”不能为 null 或空。", nameof(name));
            if (string.IsNullOrEmpty(uuid))
                throw new ArgumentException($"“{nameof(uuid)}”不能为 null 或空。", nameof(uuid));

            Name = name;
            UUID = uuid;
        }

        public PlayerUUID(Json json)
        {
            if (json is null)
                throw new ArgumentNullException(nameof(json));

            Name = json.Name;
            UUID = json.UUID;
        }

        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("uuid")]
        public string UUID { get; }

        public class Json
        {
            public Json(string name, string uUID)
            {
                Name = name;
                UUID = uUID;
            }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("uuid")]
            public string UUID { get; set; }
        }
    }
}
