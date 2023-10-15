using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod
{
    public class ForgeModInfoParser : ModInfoParser
    {
        public override bool TryParse(Stream stream, [MaybeNullWhen(false)] out ModInfo result)
        {
            throw new NotImplementedException();
        }
    }
}
