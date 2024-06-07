using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Downloading
{
    public class VersionIndex : INetworkAssetIndex
    {
        public VersionIndex(Model model)
        {
            NullValidator.ValidateObject(model, nameof(model));

            ID = model.id;
            Type = model.type;
            Url = model.url;
            Time = model.time;
            ReleaseTime = model.releaseTime;
        }

        public string ID { get; }

        public string Type { get; }

        public string Url { get; }

        public string Time { get; }

        public string ReleaseTime { get; }

        public class Model
        {
            public required string id { get; set; }

            public required string type { get; set; }

            public required string url { get; set; }

            public required string time { get; set; }

            public required string releaseTime { get; set; }
        }
    }
}
