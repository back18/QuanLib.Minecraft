using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod.Forge
{
    public class ForgeModVersion : IDataModelOwner<ForgeModVersion, ForgeModVersion.DataModel>
    {
        public ForgeModVersion(DataModel model)
        {
            NullValidator.ValidateObject(model, nameof(model));

            ModId = model.modId;
            Mandatory = model.mandatory;
            VersionRange = model.versionRange;
            Ordering = model.ordering;
            Side = model.side;
            ReferralUrl = model.referralUrl;
        }

        public string ModId { get; }

        public bool Mandatory { get; }

        public string VersionRange { get; }

        public string Ordering { get; }

        public string Side { get; }

        public string ReferralUrl { get; }

        public override string ToString()
        {
            return $"{ModId}-{VersionRange}";
        }

        public DataModel ToDataModel()
        {
            return new()
            {
                modId = ModId,
                mandatory = Mandatory,
                versionRange = VersionRange,
                ordering = Ordering,
                side = Side,
                referralUrl = ReferralUrl
            };
        }

        public static ForgeModVersion FromDataModel(DataModel model)
        {
            return new(model);
        }

        public class DataModel : IDataModel<DataModel>
        {
            public DataModel()
            {
                modId = string.Empty;
                mandatory = true;
                versionRange = string.Empty;
                ordering = "NONE";
                side = "BOTH";
                referralUrl = string.Empty;
            }

            public string modId { get; set; }

            public bool mandatory { get; set; }

            public string versionRange { get; set; }

            public string ordering { get; set; }

            public string side { get; set; }

            public string referralUrl { get; set; }

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
