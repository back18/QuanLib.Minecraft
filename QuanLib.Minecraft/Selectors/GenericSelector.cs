using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Selectors
{
    public class GenericSelector : Selector
    {
        public GenericSelector(string target)
        {
            ArgumentException.ThrowIfNullOrEmpty(target, nameof(target));

            Target = target;
        }

        public string Target { get; }

        public override string ToString()
        {
            return Target;
        }
    }
}
