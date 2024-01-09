using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod.Fabric
{
    public enum ModDependencyKind
    {
        Depends,

        Recommends,

        Suggests,

        Conflicts,

        Breaks
    }
}
