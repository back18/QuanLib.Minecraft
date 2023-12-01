using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.GameResource
{
    public static class Extension
    {
        public static async Task<byte[]> DownloadBytesAsync(this INetworkAssetIndex source, DownloadProvider? downloadProvider = null)
        {
            return await DownloadUtil.DownloadBytesAsync(source.Url, downloadProvider);
        }

        public static async Task DownloadFileAsync(this IDownloadAssetIndex source, string rootPath, DownloadProvider? downloadProvider = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(rootPath, nameof(rootPath));

            string path = Path.Combine(rootPath, source.Path);
            await DownloadUtil.DownloadFileAsync(path, source.Url, downloadProvider);
        }

        public static async Task DownloadFileAsync(this IDownloadAssetIndex source, DownloadProvider? downloadProvider = null)
        {
            await DownloadUtil.DownloadFileAsync(source.Url, downloadProvider);
        }
    }
}
