using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod
{
    public class FabricModInfoReader : ModInfoReader
    {
        public override bool TryRead(Stream stream, [MaybeNullWhen(false)] out ModInfo result)
        {
            throw new NotImplementedException();
        }
    }
}
