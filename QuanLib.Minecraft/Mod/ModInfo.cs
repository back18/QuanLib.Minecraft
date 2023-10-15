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
        public abstract ModLoaderType ModLoaderType { get; }

        public abstract string ID { get; }

        public abstract string Name { get; }

        public abstract string Description { get; }

        public abstract string Version { get; }

        public abstract string License { get; }

        public abstract string Icon { get; }

        public override string ToString()
        {
            return $"{ID}-{ModLoaderType}-{Version}";
        }
    }
}
