using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod.Forge
{
    public class ForgeModFileInfo : IDataModelOwner<ForgeModFileInfo, ForgeModFileInfo.DataModel>
    {
        public ForgeModFileInfo(DataModel model)
        {
            NullValidator.ValidateObject(model, nameof(model));

            ModLoader = model.modLoader;
            LoaderVersion = model.loaderVersion;
            License = model.license;
            IssueTrackerURL = model.issueTrackerURL;
            ShowAsDataPack = model.showAsDataPack;
            ShowAsResourcePack = model.showAsResourcePack;
            ClientSideOnly = model.clientSideOnly;
            Services = model.services.AsReadOnly();
            Properties = model.properties.AsReadOnly();
            ModInfos = model.mods.Select(s => ForgeModInfo.FromDataModel(s)).ToArray().AsReadOnly();
        }

        public string ModLoader { get; }

        public string LoaderVersion { get; }

        public string License { get; }

        public string IssueTrackerURL { get; }

        public bool ShowAsDataPack { get; }

        public bool ShowAsResourcePack { get; }

        public bool ClientSideOnly { get; }

        public ReadOnlyCollection<string> Services { get; }

        public ReadOnlyDictionary<string, object> Properties { get; }

        public ReadOnlyCollection<ForgeModInfo> ModInfos { get; }

        public DataModel ToDataModel()
        {
            return new()
            {
                modLoader = ModLoader,
                loaderVersion = LoaderVersion,
                license = License,
                issueTrackerURL = IssueTrackerURL,
                showAsDataPack = ShowAsDataPack,
                showAsResourcePack = ShowAsResourcePack,
                clientSideOnly = ClientSideOnly,
                services = Services.ToArray(),
                properties = Properties.ToDictionary(),
                mods = ModInfos.Select(s => s.ToDataModel()).ToArray()
            };
        }

        public static ForgeModFileInfo FromDataModel(DataModel model)
        {
            return new(model);
        }

        public class DataModel : IDataModel<DataModel>
        {
            public DataModel()
            {
                modLoader = string.Empty;
                loaderVersion = string.Empty;
                license = string.Empty;
                issueTrackerURL = string.Empty;
                showAsDataPack = false;
                showAsResourcePack = false;
                clientSideOnly = false;
                services = [];
                properties = [];
                mods = [];
            }

            public string modLoader { get; set; }

            public string loaderVersion { get; set; }

            public string license { get; set; }

            public string issueTrackerURL { get; set; }

            public bool showAsDataPack { get; set; }

            public bool showAsResourcePack { get; set; }

            public bool clientSideOnly { get; set; }

            public string[] services { get; set; }

            public Dictionary<string, object> properties { get; set; }

            public ForgeModInfo.DataModel[] mods { get; set; }

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
