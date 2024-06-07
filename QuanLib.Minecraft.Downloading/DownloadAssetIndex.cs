using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Downloading
{
    public class DownloadAssetIndex : NetworkAssetIndex
    {
        public DownloadAssetIndex(Model model) : base(model)
        {
            Path = model.path;
        }

        public DownloadAssetIndex(string hash, int size, string url, string path) : base(hash, size, url)
        {
            ArgumentException.ThrowIfNullOrEmpty(path, nameof(path));

            Path = path;
        }

        public string Path { get; }

        public new class Model : NetworkAssetIndex.Model
        {
            public required string path { get; set; }
        }
    }
}
