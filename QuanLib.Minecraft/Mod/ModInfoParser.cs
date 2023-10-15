using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod
{
    public abstract class ModInfoParser
    {
        public abstract bool TryParse(Stream stream, [MaybeNullWhen(false)] out ModInfo result);
    }
}
