using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod.Fabric
{
    public class PersonInfo : IDataViewModel<PersonInfo>
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

        public object ToDataModel()
        {
            return new DataModel()
            {
                name = Name,
                contact = Contact.ToDictionary()
            };
        }

        public static PersonInfo FromDataModel(object model)
        {
            return new PersonInfo((DataModel)model);
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
