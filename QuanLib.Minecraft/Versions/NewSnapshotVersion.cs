using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace QuanLib.Minecraft.Versions
{
    public partial class NewSnapshotVersion : MinecraftVersion
    {
        public const string PATTERN = @"^(\d+)\.(\d+)(?:\.(\d+))?-snapshot-?(\d+)$";

        public NewSnapshotVersion(string versionNumber, DateTime releaseTime) : base(versionNumber, VersionType.NewSnapshot, releaseTime)
        {
            Match match = VersionRegex().Match(versionNumber);
            if (match.Success)
            {
                int major = int.Parse(match.Groups[1].Value);
                int minor = int.Parse(match.Groups[2].Value);

                if (match.Groups[3].Success)
                {
                    int build = int.Parse(match.Groups[3].Value);
                    Version = new Version(major, minor, build);
                }
                else
                {
                    Version = new Version(major, minor);
                }

                int snapshotVersion = int.Parse(match.Groups[4].Value);
                SnapshotVersion = snapshotVersion;
            }
            else
            {
                throw new FormatException();
            }
        }

        public override bool IsReleaseVersion => false;

        public override bool IsSnapshotVersion => true;

        public override bool IsAncientVersion => false;

        public Version Version { get; }

        public int SnapshotVersion { get; }

        [GeneratedRegex(PATTERN)]
        private static partial Regex VersionRegex();
    }
}
