using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Downloading
{
    public class MojangDownloadProvider : IMinecraftDownloadProvider
    {
        public const string RESOURCES_URL = "https://resources.download.minecraft.net/";

        public const string LIBRARIES_URL = "https://libraries.minecraft.net/";

        public const string PISTON_DATA_URL = "https://piston-data.mojang.com/";

        public const string PISTON_META_URL = "https://piston-meta.mojang.com/";

        public static readonly MojangDownloadProvider Default = new();

        public string VersionManifestUrl { get; } = PISTON_META_URL + "mc/game/version_manifest.json";

        public string AssetBaseUrl { get; } = RESOURCES_URL;

        public string GetAssetUrl(string hash)
        {
            ArgumentException.ThrowIfNullOrEmpty(hash, nameof(hash));
            ThrowHelper.StringLengthOutOfMin(2, hash, nameof(hash));

            return $"{AssetBaseUrl}{hash[..2]}/{hash}";
        }

        public string RedirectUrl(string url)
        {
            return url;
        }
    }
}
