using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod
{
    public class FabricModInfo : ModInfo
    {
        public FabricModInfo(Model model)
        {
            NullValidator.ValidateObject(model, nameof(model));

            ID = model.id;
            Name = model.name;
            Description = model.description;
            Version = model.version;
            License = model.license;
            Icon = model.icon;
            Environment = model.environment;
        }

        public override ModLoaderType ModLoaderType => ModLoaderType.Fabric;

        public override string ID { get; }

        public override string Name { get; }

        public override string Description { get; }

        public override string Version { get; }

        public override string License { get; }

        public override string Icon { get; }

        public string Environment { get; }

        public class Model
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
            public string id { get; set; }

            public string name { get; set; }

            public string description { get; set; }

            public string version { get; set; }

            public string license { get; set; }

            public string icon { get; set; }

            public string environment { get; set; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        }
    }
}
