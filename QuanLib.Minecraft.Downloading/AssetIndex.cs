using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Downloading
{
    public class AssetIndex
    {
        public AssetIndex(Model model)
        {
            NullValidator.ValidateObject(model, nameof(model));

            if (!string.IsNullOrEmpty(model.hash))
                Hash = model.hash;
            else if (!string.IsNullOrEmpty(model.sha1))
                Hash = model.sha1;
            else
                throw new ArgumentException("哈希值不能为 null 或空", nameof(model));
            Size = model.size;
        }

        public AssetIndex(string hash, int size)
        {
            ArgumentException.ThrowIfNullOrEmpty(hash, nameof(hash));

            Hash = hash;
            Size = size;
        }

        public string Hash { get; }

        public int Size { get; }

        public class Model
        {
            [Nullable]
            public string? hash { get; set; }

            [Nullable]
            public string? sha1 { get; set; }

            public required int size { get; set; }
        }
    }
}
