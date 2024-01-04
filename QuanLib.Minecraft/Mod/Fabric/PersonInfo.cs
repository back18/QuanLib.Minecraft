using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod.Fabric
{
    public class PersonInfo : IDataModelOwner<PersonInfo, PersonInfo.DataModel>
    {
        public PersonInfo(DataModel model)
        {
            NullValidator.ValidateObject(model, nameof(model));

            Name = model.name;
            Contact = model.contact.AsReadOnly();
        }

        public string Name { get; }

        public ReadOnlyDictionary<string, string> Contact { get; }

        public override string ToString()
        {
            return Name;
        }

        public DataModel ToDataModel()
        {
            return new()
            {
                name = Name,
                contact = Contact.ToDictionary()
            };
        }

        public static PersonInfo FromDataModel(DataModel model)
        {
            return new(model);
        }

        public class DataModel : IDataModel<DataModel>
        {
            public DataModel()
            {
                name = string.Empty;
                contact = [];
            }

            public string name { get; set; }

            public Dictionary<string, string> contact { get; set; }

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
