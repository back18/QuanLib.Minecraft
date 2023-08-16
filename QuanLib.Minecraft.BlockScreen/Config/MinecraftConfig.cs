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

            MinecraftServerMode = json.MinecraftServerMode;
            ServerPath = json.ServerPath;
            ServerAddress = json.ServerAddress;
            JavaPath = json.JavaPath;
            LaunchArguments = json.LaunchArguments;
            AccelerationEngineEventPort = json.AccelerationEngineEventPort;
            AccelerationEngineDataPort = json.AccelerationEngineDataPort;
            BlockTextureBlacklist = json.BlockTextureBlacklist;
        }

        public MinecraftServerMode MinecraftServerMode { get; }

        public string ServerPath { get; }

        public string ServerAddress { get; }

        public string JavaPath { get; }

        public string LaunchArguments { get; }

        public ushort AccelerationEngineEventPort { get; }

        public ushort AccelerationEngineDataPort { get; }

        public IReadOnlyList<string> BlockTextureBlacklist { get; }

        public static MinecraftConfig Load(string path)
        {
            return new(JsonConvert.DeserializeObject<Json>(File.ReadAllText(path)) ?? throw new FormatException());
        }

        public class Json
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
            public MinecraftServerMode MinecraftServerMode;

            public string ServerPath;

            public string ServerAddress;

            public string JavaPath;

            public string LaunchArguments;

            public ushort AccelerationEngineEventPort;

            public ushort AccelerationEngineDataPort;

            public string[] BlockTextureBlacklist;
        }
    }
}
