using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.GameResource
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
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
            public string path { get; set; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        }
    }
}
