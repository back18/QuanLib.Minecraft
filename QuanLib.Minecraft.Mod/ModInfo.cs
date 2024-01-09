using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod
{
    public abstract class ModInfo
    {
        public abstract ModLoader ModLoader { get; }

        public abstract string ModId { get; }

        public abstract string ModName { get; }

        public abstract string Description { get; }

        public abstract string Version { get; }

        public abstract string LogoFile { get; }

        public override string ToString()
        {
            return $"{ModLoader} - {ModId} ({Version})";
        }
    }
}
