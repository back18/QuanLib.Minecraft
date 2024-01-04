using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod.Forge.JarMetadata
{
    public struct Version
    {
        public required string Range { get; set; }

        public required string ArtifactVersion { get; set; }
    }
}
