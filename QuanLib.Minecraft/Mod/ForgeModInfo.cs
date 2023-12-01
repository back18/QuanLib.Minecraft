using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod
{
    public class ForgeModInfo : ModInfo
    {
        public ForgeModInfo(Model model)
        {
            ArgumentNullException.ThrowIfNull(model, nameof(model));

            NullValidator.ValidateObject(model, nameof(model));
            string name = nameof(model) + ".mods[0]";
            ModModel? modModel = model.mods.FirstOrDefault() ?? throw new ArgumentException($"“{name}”不能为 null 或空。", name);
            NullValidator.ValidateObject(modModel, name);

            ID = modModel.modId;
            Name = modModel.displayName;
            Description = modModel.description;
            Version = modModel.version;
            License = model.license;
            Icon = model.logoFile ?? string.Empty;
        }

        public override ModLoaderType ModLoaderType => ModLoaderType.Forge;

        public override string ID { get; }

        public override string Name { get; }

        public override string Description { get; }

        public override string Version { get; }

        public override string License { get; }

        public override string Icon { get; }

        public class Model
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
            public string license { get; set; }

            [Nullable]
            public string? logoFile { get; set; }

            public ModModel[] mods { get; set; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        }

        public class ModModel
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
            public string modId { get; set; }

            public string displayName { get; set; }

            public string description { get; set; }

            public string version { get; set; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        }
    }
}
