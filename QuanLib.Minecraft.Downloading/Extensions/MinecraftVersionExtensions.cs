using QuanLib.Minecraft.Versions;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLib.Minecraft.Downloading.Extensions
{
    public static class MinecraftVersionExtensions
    {
        public static MinecraftVersion ParseVersion(this VersionIndex versionIndex)
        {
            ArgumentNullException.ThrowIfNull(versionIndex, nameof(versionIndex));

            DateTime releaseTime = DateTimeOffset.Parse(versionIndex.ReleaseTime).UtcDateTime;
            VersionType versionType = VersionFactory.Parse(versionIndex.Id, versionIndex.Type, releaseTime);

            return VersionFactory.Create(versionIndex.Id, versionType, releaseTime);
        }

        public static MinecraftVersion ParseVersion(this VersionJsonPatch versionPatch)
        {
            ArgumentNullException.ThrowIfNull(versionPatch, nameof(versionPatch));
            if (versionPatch.Id != "game")
                throw new ArgumentException($"The version patche with id '{versionPatch.Id}' is not a game version patche.", nameof(versionPatch));

            string versionNumber = versionPatch.Version;
            DateTime releaseTime = DateTimeOffset.Parse(versionPatch.ReleaseTime).UtcDateTime;
            VersionType versionType = VersionFactory.Parse(versionNumber, versionPatch.Type, releaseTime);

            return VersionFactory.Create(versionNumber, versionType, releaseTime);
        }

        extension(VersionList)
        {
            public static VersionList LoadInstanceFromManifest(VersionManifest versionManifest)
            {
                ArgumentNullException.ThrowIfNull(versionManifest, nameof(versionManifest));

                List<MinecraftVersion.Model> models = [];
                foreach (var versionIndex in versionManifest.Values)
                {
                    DateTime releaseTime = DateTimeOffset.Parse(versionIndex.ReleaseTime).UtcDateTime;
                    VersionType versionType = VersionFactory.Parse(versionIndex.Id, versionIndex.Type, releaseTime);
                    MinecraftVersion.Model model = new()
                    {
                        Version = versionIndex.Id,
                        Type = versionType.ToString(),
                        ReleaseTime = releaseTime.Ticks
                    };

                    models.Add(model);
                }

                return VersionList.LoadInstance(new(models));
            }
        }
    }
}
