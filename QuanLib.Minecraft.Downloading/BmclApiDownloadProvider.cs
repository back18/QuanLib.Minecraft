using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Downloading
{
    public class BmclApiDownloadProvider : IMinecraftDownloadProvider
    {
        public const string ROOT_URL = "https://bmclapi2.bangbang93.com/";

        public const string RESOURCES_URL = ROOT_URL + "assets/";

        public const string LIBRARIES_URL = ROOT_URL + "libraries/";

        public static readonly BmclApiDownloadProvider Default = new();

        public string VersionManifestUrl { get; } = ROOT_URL + "mc/game/version_manifest.json";

        public string AssetBaseUrl { get; } = RESOURCES_URL;

        public string GetAssetUrl(string hash)
        {
            ArgumentException.ThrowIfNullOrEmpty(hash, nameof(hash));
            ThrowHelper.StringLengthOutOfMin(2, hash, nameof(hash));

            return $"{AssetBaseUrl}{hash[..2]}/{hash}";
        }

        public string RedirectUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return url;
            }
            else if (url.StartsWith(MojangDownloadProvider.RESOURCES_URL))
            {
                return url.Replace(MojangDownloadProvider.RESOURCES_URL, RESOURCES_URL);
            }
            else if (url.StartsWith(MojangDownloadProvider.LIBRARIES_URL))
            {
                return url.Replace(MojangDownloadProvider.LIBRARIES_URL, LIBRARIES_URL);
            }
            else if (url.StartsWith(MojangDownloadProvider.PISTON_DATA_URL))
            {
                return url.Replace(MojangDownloadProvider.PISTON_DATA_URL, ROOT_URL);
            }
            else if (url.StartsWith(MojangDownloadProvider.PISTON_META_URL))
            {
                return url.Replace(MojangDownloadProvider.PISTON_META_URL, ROOT_URL);
            }
            else
            {
                return url;
            }
        }
    }
}
