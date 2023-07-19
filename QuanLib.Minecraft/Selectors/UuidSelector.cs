using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Selectors
{
    public class UuidSelector : Selector
    {
        UuidSelector(Guid uuid)
        {
            UUID = uuid;
        }

        public Guid UUID { get; }

        public override string ToString()
        {
            return UUID.ToString();
        }
    }
}
