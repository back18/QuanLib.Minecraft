using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Config
{
    public class SystemConfig
    {
        public SystemConfig(Json json)
        {
            if (json is null)
                throw new ArgumentNullException(nameof(json));
;
            ServicesAppID = json.ServicesAppID;
            StartupChecklist = json.StartupChecklist;
        }
        public string ServicesAppID { get; }

        public IReadOnlyList<string> StartupChecklist { get; }

        public static SystemConfig Load(string path)
        {
            return new(JsonConvert.DeserializeObject<Json>(File.ReadAllText(path)) ?? throw new FormatException());
        }

        public class Json
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
            public string ServicesAppID;

            public string[] StartupChecklist;
        }
    }
}
