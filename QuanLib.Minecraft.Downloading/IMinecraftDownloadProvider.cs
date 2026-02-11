using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLib.Minecraft.Downloading
{
    public interface IMinecraftDownloadProvider
    {
        public string VersionManifestUrl { get; }

        public string AssetBaseUrl { get; }

        public string GetAssetUrl(string hash);

        public string RedirectUrl(string url);
    }
}
