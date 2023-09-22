using Newtonsoft.Json;
using QuanLib.Core.ExceptionHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.GameResource
{
    public abstract class DownloadProvider
    {
        public static readonly MojangDownloadProvider MOJANG_PROVIDER = new();

        public static readonly BmclApiDownloadProvider BMCLAPI_PROVIDER = new();

        public abstract string VersionListUrl { get; }

        public abstract string AssetBaseUrl { get; }

        public virtual string ToAssetUrl(string hash)
        {
            if (string.IsNullOrEmpty(hash))
                throw new ArgumentException($"“{nameof(hash)}”不能为 null 或空。", nameof(hash));
            ThrowHelper.StringLengthOutOfMin(2, hash, nameof(hash));

            return $"{AssetBaseUrl}{hash[..2]}/{hash}";
        }

        public virtual string RedirectUrl(string url)
        {
            return url;
        }
    }
}
