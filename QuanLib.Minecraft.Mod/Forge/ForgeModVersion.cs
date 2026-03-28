using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod.Forge
{
    public class ForgeModVersion : IDataViewModel<ForgeModVersion>
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

        public object ToDataModel()
        {
            return new DataModel()
            {
                modId = ModId,
                mandatory = Mandatory,
                versionRange = VersionRange,
                ordering = Ordering,
                side = Side,
                referralUrl = ReferralUrl
            };
        }

        public static ForgeModVersion FromDataModel(object model)
        {
            return new ForgeModVersion((DataModel)model);
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
