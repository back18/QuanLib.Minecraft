using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.GameResource
{
    public class BmclApiDownloadProvider : DownloadProvider
    {
        public const string ROOT_URL = "https://bmclapi2.bangbang93.com/";

        public const string RESOURCES_DOWNLOAD_URL = ROOT_URL + "assets/";

        public BmclApiDownloadProvider()
        {
            VersionListUrl = ROOT_URL + "mc/game/version_manifest.json";
            AssetBaseUrl = RESOURCES_DOWNLOAD_URL;
        }

        public override string VersionListUrl { get; }

        public override string AssetBaseUrl { get; }

        public override string RedirectUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return url;
            else if (url.StartsWith(MojangDownloadProvider.PISTON_DATA_URL))
                return url.Replace(MojangDownloadProvider.PISTON_DATA_URL, ROOT_URL);
            else if (url.StartsWith(MojangDownloadProvider.RESOURCES_DOWNLOAD_URL))
                return url.Replace(MojangDownloadProvider.RESOURCES_DOWNLOAD_URL, RESOURCES_DOWNLOAD_URL);
            else
                return url;
        }
    }
}
