using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Versions
{
    public class ReleaseVersion(string versionNumber, DateTime releaseTime) : MinecraftVersion(versionNumber, VersionType.Release, releaseTime)
    {
        public override bool IsReleaseVersion => true;

        public override bool IsSnapshotVersion => false;

        public override bool IsAncientVersion => false;

        public Version Version { get; } = new Version(versionNumber);
    }
}
