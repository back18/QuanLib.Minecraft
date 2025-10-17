using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace QuanLib.Minecraft.Versions.Extensions
{
    public static class MinecraftVersionExtension
    {
        private static MinecraftVersion? _version_1_18;
        private static MinecraftVersion? _version_1_2_1;

        public static ReleaseVersion? GetPreviousReleaseVersion(this MinecraftVersion source)
        {
            ArgumentNullException.ThrowIfNull(source, nameof(source));

            var releaseVersions = VersionList.Instance.ReleaseVersions;
            DateTime releaseTime = source.ReleaseTime;

            for (int i = releaseVersions.Count - 1; i >= 0; i--)
            {
                ReleaseVersion releaseVersion = releaseVersions[i];
                if (releaseVersion.ReleaseTime < releaseTime)
                    return releaseVersion;
            }

            return null;
        }

        public static ReleaseVersion? GetNextReleaseVersion(this MinecraftVersion source)
        {
            ArgumentNullException.ThrowIfNull(source, nameof(source));

            var releaseVersions = VersionList.Instance.ReleaseVersions;
            DateTime releaseTime = source.ReleaseTime;

            for (int i = 0; i < releaseVersions.Count; i++)
            {
                ReleaseVersion releaseVersion = releaseVersions[i];
                if (releaseVersion.ReleaseTime > releaseTime)
                    return releaseVersion;
            }

            return null;
        }

        public static int GetOverworldMaxBuildingHeight(this MinecraftVersion source)
        {
            ArgumentNullException.ThrowIfNull(source, nameof(source));

            _version_1_18 ??= VersionList.Instance.GetVersion("1.18");
            _version_1_2_1 ??= VersionList.Instance.GetVersion("1.2.1");
            if (source >= _version_1_18)
                return 319;
            else if (source >= _version_1_2_1)
                return 255;
            else
                return 127;
        }

        public static int GetOverworldMinBuildingHeight(this MinecraftVersion source)
        {
            ArgumentNullException.ThrowIfNull(source, nameof(source));

            _version_1_18 ??= VersionList.Instance.GetVersion("1.18");
            if (source >= _version_1_18)
                return -64;
            else
                return 0;
        }
    }
}
