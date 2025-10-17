using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Versions
{
    public class OldPreReleaseVersion(string versionNumber, DateTime releaseTime) : MinecraftVersion(versionNumber, VersionType.OldPreRelease, releaseTime)
    {
        public Version Version { get; } = new Version(versionNumber);

        public override bool IsReleaseVersion => false;

        public override bool IsSnapshotVersion => true;

        public override bool IsAncientVersion => false;
    }
}
