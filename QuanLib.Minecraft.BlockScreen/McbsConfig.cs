using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public class McbsConfig
    {
        public McbsConfig(Json json)
        {
            if (json is null)
                throw new ArgumentNullException(nameof(json));

            ServerPath = json.ServerPath;
            ServerAddress = json.ServerAddress;
            AccelerationEngineEventPort = json.AccelerationEngineEventPort;
            AccelerationEngineDataPort = json.AccelerationEngineDataPort;
            ServicesAppID = json.ServicesAppID;
            StartupChecklist = json.StartupChecklist;
            CursorReaderObjective = json.CursorReaderObjective;
        }

        public string ServerPath { get;}

        public string ServerAddress { get; }

        public int AccelerationEngineEventPort { get; }

        public int AccelerationEngineDataPort { get; }

        public string ServicesAppID { get; }

        public IReadOnlyList<string> StartupChecklist { get; }

        public string CursorReaderObjective { get; }

        public class Json
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
            public string ServerPath { get; set; }

            public string ServerAddress { get; set; }

            public int AccelerationEngineEventPort { get; set; }

            public int AccelerationEngineDataPort { get; set; }

            public string ServicesAppID { get; set; }

            public string[] StartupChecklist { get; set; }

            public string CursorReaderObjective { get; set; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        }
    }
}
