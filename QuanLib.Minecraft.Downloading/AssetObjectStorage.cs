using QuanLib.Core;
using QuanLib.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace QuanLib.Minecraft.Downloading
{
    public class AssetObjectStorage
    {
        public AssetObjectStorage(string objectsFolder, HashType hashType = HashType.SHA1)
        {
            ArgumentException.ThrowIfNullOrEmpty(objectsFolder, nameof(objectsFolder));

            ObjectsFolder = Path.GetFullPath(objectsFolder);
            HashType = hashType;
        }

        public string ObjectsFolder { get; }

        public HashType HashType { get; }

        public bool Exists([NotNullWhen(true)] string? hash)
        {
            if (string.IsNullOrEmpty(hash))
                return false;

            if (hash.Length != HashType.GetHashSizeInChars())
                return false;

            string folder = Path.Combine(ObjectsFolder, hash[..2]);
            if (!Directory.Exists(folder))
                return false;

            string file = Path.Combine(folder, hash);
            return File.Exists(file);
        }

        public bool Validate([NotNullWhen(true)] string? hash, int size)
        {
            if (size < 0)
                return false;

            if (string.IsNullOrEmpty(hash))
                return false;

            if (hash.Length != HashType.GetHashSizeInChars())
                return false;

            string folder = Path.Combine(ObjectsFolder, hash[..2]);
            if (!Directory.Exists(folder))
                return false;

            FileInfo fileInfo = new(Path.Combine(folder, hash));
            if (!fileInfo.Exists)
                return false;

            if (fileInfo.Length != size)
                return false;

            return HashUtil.GetHashString(fileInfo.FullName, HashType).Equals(hash, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<bool> ValidateAsync([NotNullWhen(true)] string? hash, int size, CancellationToken cancellationToken = default)
        {
            if (size < 0)
                return false;

            if (string.IsNullOrEmpty(hash))
                return false;

            if (hash.Length != HashType.GetHashSizeInChars())
                return false;

            string folder = Path.Combine(ObjectsFolder, hash[..2]);
            if (!Directory.Exists(folder))
                return false;

            FileInfo fileInfo = new(Path.Combine(folder, hash));
            if (!fileInfo.Exists)
                return false;

            if (fileInfo.Length != size)
                return false;

            string fileHash = await HashUtil.GetHashStringAsync(fileInfo.FullName, HashType, cancellationToken);
            return fileHash.Equals(hash, StringComparison.OrdinalIgnoreCase);
        }

        public string GetFullPath(string hash)
        {
            ArgumentException.ThrowIfNullOrEmpty(hash, nameof(hash));
            if (hash.Length != HashType.GetHashSizeInChars())
                throw new ArgumentException($"哈希值的长度必须为{HashType.GetHashSizeInChars()}");

            return Path.Combine(ObjectsFolder, hash[..2], hash);
        }

        public FileStream OpenRead(string hash)
        {
            string filePath = GetFullPath(hash);
            return File.OpenRead(filePath);
        }

        public StreamReader OpenText(string hash)
        {
            string filePath = GetFullPath(hash);
            return File.OpenText(filePath);
        }

        public StreamReader OpenText(string hash, Encoding encoding)
        {
            string filePath = GetFullPath(hash);
            return new StreamReader(filePath, encoding);
        }

        public byte[] ReadAllBytes(string hash)
        {
            string filePath = GetFullPath(hash);
            return File.ReadAllBytes(filePath);
        }

        public async Task<byte[]> ReadAllBytesAsync(string hash)
        {
            string filePath = GetFullPath(hash);
            return await File.ReadAllBytesAsync(filePath);
        }

        public string ReadAllText(string hash)
        {
            string filePath = GetFullPath(hash);
            return File.ReadAllText(filePath);
        }

        public async Task<string> ReadAllTextAsync(string hash)
        {
            string filePath = GetFullPath(hash);
            return await File.ReadAllTextAsync(filePath);
        }

        public string ReadAllText(string hash, Encoding encoding)
        {
            string filePath = GetFullPath(hash);
            return File.ReadAllText(filePath, encoding);
        }

        public async Task<string> ReadAllTextAsync(string hash, Encoding encoding)
        {
            string filePath = GetFullPath(hash);
            return await File.ReadAllTextAsync(filePath, encoding);
        }
    }
}
