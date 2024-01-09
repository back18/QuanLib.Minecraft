using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod.Fabric
{
    public class FabricMod
    {
        public FabricMod(FabricModFile owner, FabricModInfo modInfo)
        {
            ArgumentNullException.ThrowIfNull(owner, nameof(owner));
            ArgumentNullException.ThrowIfNull(modInfo, nameof(modInfo));

            Owner = owner;
            ModInfo = modInfo;
        }

        public FabricModFile Owner { get; }

        public FabricModInfo ModInfo { get; }

        public override string ToString()
        {
            return $"{Owner.ModSource} - {Owner.GetFileName()} - {ModInfo.ModId} ({ModInfo.Version})";
        }
    }
}
