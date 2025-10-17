using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Versions
{
    public class AncientVersion(string versionNumber, VersionType type, DateTime releaseTime) : MinecraftVersion(versionNumber, type, releaseTime)
    {
        public override bool IsReleaseVersion => false;

        public override bool IsSnapshotVersion => false;

        public override bool IsAncientVersion => true;
    }
}
