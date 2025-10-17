using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Versions
{
    public partial class PreReleaseVersion : MinecraftVersion
    {
        public const string PATTERN = @"^(\d+)\.(\d+)(?:\.(\d+))?-pre(\d+)$";

        public PreReleaseVersion(string versionNumber, DateTime releaseTime) : base(versionNumber, VersionType.PreRelease, releaseTime)
        {
            ArgumentException.ThrowIfNullOrEmpty(versionNumber, nameof(versionNumber));

            string input = versionNumber.Replace(" Pre-Release ", "-pre");

            Match match = VersionRegex().Match(input);
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

                int preVersion = int.Parse(match.Groups[4].Value);
                PreVersion = preVersion;
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

        public int PreVersion { get; }

        [GeneratedRegex(PATTERN)]
        private static partial Regex VersionRegex();
    }
}
