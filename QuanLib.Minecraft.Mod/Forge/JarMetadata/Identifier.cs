using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod.Forge.JarMetadata
{
    public struct Identifier
    {
        public required string Group { get; set; }

        public required string Artifact { get; set; }
    }
}
