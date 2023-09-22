using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.GameResource
{
    public static class DownloadUtil
    {
        public static async Task<byte[]> DownloadBytesAsync(string url, DownloadProvider? downloadProvider = null)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException($"“{nameof(url)}”不能为 null 或空。", nameof(url));

            using HttpClient httpClient = new();
            url = downloadProvider?.RedirectUrl(url) ?? url;
            return await httpClient.GetByteArrayAsync(url);
        }

        public static async Task DownloadFileAsync(string path, string url, DownloadProvider? downloadProvider = null)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException($"“{nameof(path)}”不能为 null 或空。", nameof(path));
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException($"“{nameof(url)}”不能为 null 或空。", nameof(url));

            byte[] bytes = await DownloadBytesAsync(url, downloadProvider);
            await File.WriteAllBytesAsync(path, bytes);
        }

        public static async Task DownloadFileAsync(string url, DownloadProvider? downloadProvider = null)
        {
            await DownloadFileAsync(Directory.GetCurrentDirectory(), url, downloadProvider);
        }
    }
}
