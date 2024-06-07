using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Downloading
{
    public class NetworkAssetIndex : AssetIndex, INetworkAssetIndex
    {
        public NetworkAssetIndex(Model model) : base(model)
        {
            Url = model.url;
        }

        public NetworkAssetIndex(string hash, int size, string url) : base(hash, size)
        {
            ArgumentException.ThrowIfNullOrEmpty(url, nameof(url));

            Url = url;
        }

        public string Url { get; }

        public new class Model : AssetIndex.Model
        {
            public required string url { get; set; }
        }
    }
}
