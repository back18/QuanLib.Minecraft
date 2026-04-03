using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLib.Minecraft
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OptionAttribute : Attribute
    {
        public OptionAttribute(string name)
        {
            ArgumentException.ThrowIfNullOrEmpty(name, nameof(name));

            Name = name;
        }

        public string Name { get; }
    }
}
