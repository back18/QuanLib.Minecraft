using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Config
{
    public class MinecraftConfig
    {
        public MinecraftConfig(Json json)
        {
            if (json is null)
                throw new ArgumentNullException(nameof(json));

            ServerPath = json.ServerPath;
            ServerAddress = json.ServerAddress;
            AccelerationEngineEventPort = json.AccelerationEngineEventPort;
            AccelerationEngineDataPort = json.AccelerationEngineDataPort;
        }

        public string ServerPath { get; }

        public string ServerAddress { get; }

        public int AccelerationEngineEventPort { get; }

        public int AccelerationEngineDataPort { get; }

        public static MinecraftConfig Load(string path)
        {
            return new(JsonConvert.DeserializeObject<Json>(File.ReadAllText(path)) ?? throw new FormatException());
        }

        public class Json
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
            public string ServerPath;

            public string ServerAddress;

            public int AccelerationEngineEventPort;

            public int AccelerationEngineDataPort;
        }
    }
}
