using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace QuanLib.Minecraft.Versions
{
    public static class VersionFactory
    {
        public static VersionType Parse(string versionNumber, string versionType)
        {
            switch (versionType)
            {
                case "release":
                    return VersionType.Release;
                case "snapshot":
                    if (AprilFoolsDayVersion.IsAprilFoolsDayVersion(versionNumber))
                        return VersionType.AprilFoolsDay;
                    else if (versionNumber.Contains("pre") || versionNumber.Contains("Pre-Release"))
                        return VersionType.PreRelease;
                    else if (versionNumber.Contains("rc"))
                        return VersionType.ReleaseCandidate;
                    else if (versionNumber.Contains("snapshot"))
                        return VersionType.NewSnapshot;
                    else if (versionNumber.Contains('w'))
                        return VersionType.Snapshot;
                    else if (versionNumber.Contains('.'))
                        return VersionType.OldPreRelease;
                    else
                        throw new FormatException($"Unknown snapshot version format: {versionNumber}");
                case "old_beta":
                    return VersionType.OldBeta;
                case "old_alpha":
                    if (versionNumber.StartsWith('a'))
                        return VersionType.OldAlpha;
                    else if (versionNumber.StartsWith("inf"))
                        return VersionType.Infdev;
                    else if (versionNumber.StartsWith('c'))
                        return VersionType.Classic;
                    else if (versionNumber.StartsWith("rd"))
                        return VersionType.PreClassic;
                    else
                        throw new FormatException($"Unknown old_alpha version format: {versionNumber}");
                default:
                    throw new FormatException($"Unknown version type: {versionType}");
            }
        }

        public static MinecraftVersion Create(string versionNumber, VersionType versionType, DateTime releaseTime)
        {
            return versionType switch
            {
                VersionType.Release => new ReleaseVersion(versionNumber, releaseTime),
                VersionType.ReleaseCandidate => new ReleaseCandidateVersion(versionNumber, releaseTime),
                VersionType.PreRelease => new PreReleaseVersion(versionNumber, releaseTime),
                VersionType.OldPreRelease => new OldPreReleaseVersion(versionNumber, releaseTime),
                VersionType.Snapshot => new SnapshotVersion(versionNumber, releaseTime),
                VersionType.NewSnapshot => new NewSnapshotVersion(versionNumber, releaseTime),
                VersionType.AprilFoolsDay => new AprilFoolsDayVersion(versionNumber, releaseTime),
                VersionType.OldBeta or
                VersionType.OldAlpha or
                VersionType.Infdev or
                VersionType.Indev or
                VersionType.Classic or
                VersionType.PreClassic => new AncientVersion(versionNumber, versionType, releaseTime),
                _ => throw new InvalidEnumArgumentException(nameof(versionType), (int)versionType, typeof(VersionType)),
            };
        }
    }
}
