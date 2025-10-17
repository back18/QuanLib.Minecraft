using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Versions
{
    public partial class SnapshotVersion : MinecraftVersion
    {
        public const string PATTERN = @"^(\d+)w(\d+)([a-z])$";

        public SnapshotVersion(string versionNumber, DateTime releaseTime) : base(versionNumber, VersionType.Snapshot, releaseTime)
        {
            Match match = VersionRegex().Match(versionNumber);

            if (match.Success)
            {
                Year = int.Parse(match.Groups[1].Value);
                Week = int.Parse(match.Groups[2].Value);
                PatchVersion = char.ToLower(char.Parse(match.Groups[3].Value)) - 'a' + 1;
            }
            else
            {
                throw new FormatException();
            }
        }

        public override bool IsReleaseVersion => false;

        public override bool IsSnapshotVersion => true;

        public override bool IsAncientVersion => false;

        public int Year { get; }

        public int Week { get; }

        public int PatchVersion { get; }

        [GeneratedRegex(PATTERN)]
        private static partial Regex VersionRegex();
    }
}
