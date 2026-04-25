using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLib.Minecraft.Stats
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class CustomStatsKeyAttribute : Attribute
    {
        public CustomStatsKeyAttribute(string key)
        {
            ArgumentNullException.ThrowIfNull(key, nameof(key));

            Key = key;
        }

        public string Key { get; }
    }
}
