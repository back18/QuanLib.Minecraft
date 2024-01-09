using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Downloading
{
    public class MojangDownloadProvider : DownloadProvider
    {
        public const string RESOURCES_URL = "https://resources.download.minecraft.net/";

        public const string LIBRARIES_URL = "https://libraries.minecraft.net/";

        public const string PISTON_DATA_URL = "https://piston-data.mojang.com/";

        public const string PISTON_META_URL = "https://piston-meta.mojang.com/";

        public MojangDownloadProvider()
        {
            VersionListUrl = PISTON_META_URL + "mc/game/version_manifest.json";
            AssetBaseUrl = RESOURCES_URL;
        }

        public override string VersionListUrl { get; }

        public override string AssetBaseUrl { get; }
    }
}
