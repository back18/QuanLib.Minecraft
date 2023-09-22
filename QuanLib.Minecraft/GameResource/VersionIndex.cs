﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.GameResource
{
    public class VersionIndex : INetworkAssetIndex
    {
        public VersionIndex(Model model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

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
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
            public string id { get; set; }

            public string type { get; set; }

            public string url { get; set; }

            public string time { get; set; }

            public string releaseTime { get; set; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        }
    }
}
