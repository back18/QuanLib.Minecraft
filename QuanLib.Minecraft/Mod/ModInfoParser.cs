using QuanLib.Core.IO;
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
        public abstract string ModInfoPath { get; }

        public abstract bool TryParse(ZipPack zipPack, [MaybeNullWhen(false)] out ModInfo result);
    }
}
