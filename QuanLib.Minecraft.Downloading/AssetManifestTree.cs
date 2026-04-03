using QuanLib.Core;
using QuanLib.IO;
using QuanLib.IO.FileSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace QuanLib.Minecraft.Downloading
{
    public class AssetManifestTree : IReadOnlyFileSystem
    {
        private const char SeparatorChar = '/';

        public AssetManifestTree(AssetObjectStorage assetObjectStorage, IReadOnlyDictionary<string, AssetIndex> assetIndex)
        {
            ArgumentNullException.ThrowIfNull(assetObjectStorage, nameof(assetObjectStorage));
            ArgumentNullException.ThrowIfNull(assetIndex, nameof(assetIndex));

            _assetObjectStorage = assetObjectStorage;
            _assetIndex = assetIndex;
            _rootNode = BuildRootNode(assetIndex);
        }

        private readonly AssetObjectStorage _assetObjectStorage;
        private readonly IReadOnlyDictionary<string, AssetIndex> _assetIndex;
        private readonly DeviceNode _rootNode;

        public char PathSeparator => SeparatorChar;

        public bool DirectoryExists([NotNullWhen(true)] string? path)
        {
            return _rootNode.DirectoryNodeExists(path);
        }

        public bool FileExists([NotNullWhen(true)] string? path)
        {
            return _rootNode.FileNodeExists(path);
        }

        public string[] GetDirectoryPaths(string path)
        {
            return _rootNode.GetDirectoryNodes(path).Select(s => s.GetFullName(SeparatorChar)).ToArray();
        }

        public string[] GetFilePaths(string path)
        {
            return _rootNode.GetFileNodes(path).Select(s => s.GetFullName(SeparatorChar)).ToArray();
        }

        public Stream ReadFile(string path)
        {
            ArgumentException.ThrowIfNullOrEmpty(path, nameof(path));

            FileNode? fileNode = _rootNode.GetFileNode(path) ?? throw new FileNotFoundException($"File '{path}' not found in asset index.");
            string fullPath = fileNode.GetFullName(SeparatorChar).TrimStart(SeparatorChar);
            AssetIndex assetIndex = _assetIndex[fullPath];
            return _assetObjectStorage.OpenRead(assetIndex.Hash);
        }

        public bool TryReadFile([NotNullWhen(true)] string? path, [MaybeNullWhen(false)] out Stream outputStream)
        {
            if (string.IsNullOrEmpty(path))
                goto failed;

            FileNode? fileNode = _rootNode.GetFileNode(path);
            if (fileNode is null)
                goto failed;

            string fullPath = fileNode.GetFullName(SeparatorChar).TrimStart(SeparatorChar);
            if (!_assetIndex.TryGetValue(fullPath, out var assetIndex))
                goto failed;

            if (!_assetObjectStorage.Exists(assetIndex.Hash))
                goto failed;

            try
            {
                outputStream = _assetObjectStorage.OpenRead(assetIndex.Hash);
                return true;
            }
            catch
            {
                goto failed;
            }

            failed:
            outputStream = null;
            return false;
        }

        public string GetStoragePath(string path)
        {
            FileNode? fileNode = _rootNode.GetFileNode(path) ?? throw new FileNotFoundException($"File '{path}' not found in asset index.");
            string fullPath = fileNode.GetFullName(SeparatorChar).TrimStart(SeparatorChar);
            AssetIndex assetIndex = _assetIndex[fullPath];
            return _assetObjectStorage.GetFullPath(assetIndex.Hash);
        }

        private static DeviceNode BuildRootNode(IReadOnlyDictionary<string, AssetIndex> assetIndex)
        {
            ArgumentNullException.ThrowIfNull(assetIndex, nameof(assetIndex));

            DeviceNode rootNode = new(string.Empty, SeparatorChar);
            foreach (var item in assetIndex)
            {
                string path = item.Key;
                rootNode.CreateFileNode(path);
            }

            return rootNode;
        }
    }
}
