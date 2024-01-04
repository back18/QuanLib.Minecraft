using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod.Fabric
{
    public class ModDependency
    {
        public ModDependency(ModDependencyKind modDependencyKind, string modId, IList<string> versionRanges)
        {
            ArgumentException.ThrowIfNullOrEmpty(modId, nameof(modId));
            ArgumentNullException.ThrowIfNull(versionRanges, nameof(versionRanges));

            Kind = modDependencyKind;
            ModId = modId;
            VersionRanges = versionRanges.AsReadOnly();
        }

        public ModDependencyKind Kind { get; }

        public string ModId { get; }

        public ReadOnlyCollection<string> VersionRanges { get; }

        public override string ToString()
        {
            return $"{Kind}: {ModId} [{string.Join(" || ", VersionRanges)}]";
        }
    }
}
