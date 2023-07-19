using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Snbt
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false)]
    public class SnbtPropertyAttribute : Attribute
    {
        public SnbtPropertyAttribute(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException($"“{nameof(propertyName)}”不能为 null 或空。", nameof(propertyName));

            PropertyName = propertyName;
        }

        public string PropertyName { get; }
    }
}
