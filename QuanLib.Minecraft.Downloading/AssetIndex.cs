using QuanLib.Core;
using QuanLib.IO;
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

            if (!string.IsNullOrEmpty(model.sha1))
            {
                HashType = HashType.SHA1;
                Hash = model.sha1;
            }
            else if (!string.IsNullOrEmpty(model.hash))
            {
                int length = model.hash.Length;
                HashType = length switch
                {
                    var len when len == HashType.MD5.GetHashSizeInChars() => HashType.MD5,
                    var len when len == HashType.SHA1.GetHashSizeInChars() => HashType.SHA1,
                    var len when len == HashType.SHA256.GetHashSizeInChars() => HashType.SHA256,
                    var len when len == HashType.SHA384.GetHashSizeInChars() => HashType.SHA384,
                    var len when len == HashType.SHA512.GetHashSizeInChars() => HashType.SHA512,
                    _ => throw new ArgumentException(
                        $"无效的哈希长度: {length}。支持的哈希长度: " +
                        $"MD5({HashType.MD5.GetHashSizeInChars()}), " +
                        $"SHA1({HashType.SHA1.GetHashSizeInChars()}), " +
                        $"SHA256({HashType.SHA256.GetHashSizeInChars()}), " +
                        $"SHA384({HashType.SHA384.GetHashSizeInChars()}), " +
                        $"SHA512({HashType.SHA512.GetHashSizeInChars()})")
                };

                Hash = model.hash;
            }
            else
            {
                throw new ArgumentException("找不到哈希值");
            }

            Size = model.size;
        }

        public AssetIndex(HashType hashType, string hash, int size)
        {
            ArgumentException.ThrowIfNullOrEmpty(hash, nameof(hash));

            HashType = hashType;
            Hash = hash;
            Size = size;
        }

        public HashType HashType { get; }

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
