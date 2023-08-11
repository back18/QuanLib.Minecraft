using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Config
{
    public class ScreenConfig
    {
        public ScreenConfig(Json json)
        {
            if (json is null)
                throw new ArgumentNullException(nameof(json));

            MaxScreenCount = json.MaxScreenCount;
            MaxLength = json.MaxLength;
            MaxPixels = json.MaxPixels;
            MinY = json.MinY;
            MaxY = json.MaxY;
            ScreenBuildTimeout = json.ScreenBuildTimeout;
            RightClickObjective = json.RightClickObjective;
        }

        public int MaxScreenCount { get; }

        public int MaxLength { get; }

        public int MaxPixels { get; }

        public int MinY { get; }

        public int MaxY { get; }

        public int ScreenBuildTimeout { get; }

        public string RightClickObjective { get; }

        public static ScreenConfig Load(string path)
        {
            return new(JsonConvert.DeserializeObject<Json>(File.ReadAllText(path)) ?? throw new FormatException());
        }

        public class Json
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
            public int MaxScreenCount;

            public int MaxLength;

            public int MaxPixels;

            public int MinY;

            public int MaxY;

            public int ScreenBuildTimeout;

            public string RightClickObjective;
        }
    }
}
