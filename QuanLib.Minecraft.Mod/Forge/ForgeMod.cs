using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod.Forge
{
    public class ForgeMod
    {
        public ForgeMod(ForgeModFile owner, ForgeModInfo modInfo)
        {
            ArgumentNullException.ThrowIfNull(owner, nameof(owner));
            ArgumentNullException.ThrowIfNull(modInfo, nameof(modInfo));

            Owner = owner;
            ModInfo = modInfo;
        }

        public ForgeModFile Owner { get; }

        public ForgeModInfo ModInfo { get; }

        public override string ToString()
        {
            return $"{Owner.ModSource} - {Owner.GetFileName()} - {ModInfo.ModId} ({ModInfo.Version})";
        }
    }
}
