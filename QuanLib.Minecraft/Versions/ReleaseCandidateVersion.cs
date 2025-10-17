using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Versions
{
    public partial class ReleaseCandidateVersion : MinecraftVersion
    {
        public const string PATTERN = @"^(\d+)\.(\d+)(?:\.(\d+))?-rc(\d+)$";

        public ReleaseCandidateVersion(string versionNumber, DateTime releaseTime) : base(versionNumber, VersionType.ReleaseCandidate, releaseTime)
        {
            ArgumentException.ThrowIfNullOrEmpty(versionNumber, nameof(versionNumber));

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

                int rcVersion = int.Parse(match.Groups[4].Value);
                RcVersion = rcVersion;
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

        public int RcVersion { get; }

        [GeneratedRegex(PATTERN)]
        private static partial Regex VersionRegex();
    }
}
