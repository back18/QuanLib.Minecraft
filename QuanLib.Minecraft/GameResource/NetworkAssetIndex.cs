using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.GameResource
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
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
            public string url { get; set; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        }
    }
}
