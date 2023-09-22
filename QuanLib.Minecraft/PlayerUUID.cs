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

        public PlayerUUID(Model model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            Name = model.name;
            UUID = model.uuid;
        }

        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("uuid")]
        public string UUID { get; }

        public class Model
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
            public string name { get; set; }

            public string uuid { get; set; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        }
    }
}
