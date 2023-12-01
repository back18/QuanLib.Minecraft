using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.GameResource
{
    public static class DownloadUtil
    {
        public static async Task<byte[]> DownloadBytesAsync(string url, DownloadProvider? downloadProvider = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(url, nameof(url));

            url = downloadProvider?.RedirectUrl(url) ?? url;
            HttpClientHandler clientHandler = new();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            using HttpClient httpClient = new(clientHandler);
            return await httpClient.GetByteArrayAsync(url);
        }

        public static async Task DownloadFileAsync(string path, string url, DownloadProvider? downloadProvider = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(path, nameof(path));
            ArgumentException.ThrowIfNullOrEmpty(url, nameof(url));

            byte[] bytes = await DownloadBytesAsync(url, downloadProvider);
            await File.WriteAllBytesAsync(path, bytes);
        }

        public static async Task DownloadFileAsync(string url, DownloadProvider? downloadProvider = null)
        {
            await DownloadFileAsync(Directory.GetCurrentDirectory(), url, downloadProvider);
        }
    }
}
