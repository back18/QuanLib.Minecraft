using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.GameResource
{
    public class MojangDownloadProvider : DownloadProvider
    {
        public const string PISTON_META_URL = "https://piston-meta.mojang.com/";

        public const string PISTON_DATA_URL = "https://piston-data.mojang.com/";

        public const string RESOURCES_DOWNLOAD_URL = "https://resources.download.minecraft.net/";

        public const string LIBRARIES_URL = "https://libraries.minecraft.net/";

        public MojangDownloadProvider()
        {
            VersionListUrl = PISTON_META_URL + "/mc/game/version_manifest.json";
            AssetBaseUrl = RESOURCES_DOWNLOAD_URL;
        }

        public override string VersionListUrl { get; }

        public override string AssetBaseUrl { get; }
    }
}
