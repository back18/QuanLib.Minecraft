using QuanLib.Core.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod
{
    public class FabricModInfoParser : ModInfoParser
    {
        public override string ModInfoPath => "fabric.mod.json";

        public override bool TryParse(ZipPack zipPack, [MaybeNullWhen(false)] out ModInfo result)
        {
            throw new NotImplementedException();
        }
    }
}
