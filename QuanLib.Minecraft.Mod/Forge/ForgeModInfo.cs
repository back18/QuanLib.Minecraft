using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod.Forge
{
    public class ForgeModInfo : ModInfo, IDataModelOwner<ForgeModInfo, ForgeModInfo.DataModel>
    {
        public ForgeModInfo(DataModel model)
        {
            NullValidator.ValidateObject(model, nameof(model));

            ModId = model.modId;
            Namespace = model.@namespace;
            Version = model.version;
            ModName = model.displayName;
            Description = model.description;
            LogoFile = model.logoFile;
            LogoBlur = model.logoBlur;
            UpdateJsonUrl = model.updateJSONURL;
            ModUrl = model.modUrl;
            Dependencies = model.dependencies.Select(s => ForgeModVersion.FromDataModel(s)).ToArray().AsReadOnly();
            Features = model.features.AsReadOnly();
            ModProperties = model.modproperties.AsReadOnly();
        }

        public override ModLoader ModLoader => ModLoader.Forge;

        public override string ModId { get; }

        public string Namespace { get; }

        public override string Version { get; }

        public override string ModName { get; }

        public override string Description { get; }

        public override string LogoFile { get; }

        public bool LogoBlur { get; }

        public string UpdateJsonUrl { get; }

        public string ModUrl { get; }

        public ReadOnlyCollection<ForgeModVersion> Dependencies { get; }

        public ReadOnlyCollection<object> Features { get; }

        public ReadOnlyDictionary<string, object> ModProperties { get; }

        public DataModel ToDataModel()
        {
            return new()
            {
                modId = ModId,
                @namespace = Namespace,
                version = Version,
                displayName = ModName,
                description = Description,
                logoFile = LogoFile,
                logoBlur = LogoBlur,
                updateJSONURL = UpdateJsonUrl,
                modUrl = ModUrl,
                dependencies = Dependencies.Select(s => s.ToDataModel()).ToList(),
                features = Features.ToList(),
                modproperties = ModProperties.ToDictionary()
            };
        }

        public static ForgeModInfo FromDataModel(DataModel model)
        {
            return new(model);
        }

        public class DataModel : IDataModel<DataModel>
        {
            public DataModel()
            {
                modId = string.Empty;
                @namespace = string.Empty;
                version = string.Empty;
                displayName = string.Empty;
                description = string.Empty;
                logoFile = string.Empty;
                logoBlur = true;
                updateJSONURL = string.Empty;
                modUrl = string.Empty;
                dependencies = [];
                features = [];
                modproperties = [];
            }

            public string modId { get; set; }

            public string @namespace { get; set; }

            public string version { get; set; }

            public string displayName { get; set; }

            public string description { get; set; }

            public string logoFile { get; set; }

            public bool logoBlur { get; set; }

            public string updateJSONURL { get; set; }

            public string modUrl { get; set; }

            public List<ForgeModVersion.DataModel> dependencies { get; set; }

            public List<object> features { get; set; }

            public Dictionary<string, object> modproperties { get; set; }

            public static DataModel CreateDefault()
            {
                return new();
            }

            public static void Validate(DataModel model, string name)
            {
                ArgumentNullException.ThrowIfNull(model, nameof(model));
                ArgumentException.ThrowIfNullOrEmpty(name, nameof(name));
            }
        }
    }
}
