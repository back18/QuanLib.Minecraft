using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod.Forge
{
    public class ForgeModFileInfo : IDataViewModel<ForgeModFileInfo>
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

        public object ToDataModel()
        {
            return new DataModel()
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
                mods = ModInfos.Select(s => (ForgeModInfo.DataModel)s.ToDataModel()).ToArray()
            };
        }

        public static ForgeModFileInfo FromDataModel(object model)
        {
            return new ForgeModFileInfo((DataModel)model);
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
                return new DataModel();
            }

            public IValidatableObject GetValidator()
            {
                return new ValidatableObject(this);
            }

            public IEnumerable<IValidatable> GetValidatableProperties()
            {
                return Enumerable.Empty<IValidatable>();
            }
        }
    }
}
