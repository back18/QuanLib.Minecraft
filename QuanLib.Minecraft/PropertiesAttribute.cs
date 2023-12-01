using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class PropertiesAttribute : Attribute
    {
        public PropertiesAttribute(string name)
        {
            ArgumentException.ThrowIfNullOrEmpty(name, nameof(name));

            Name = name;
        }

        public string Name { get; }
    }
}
