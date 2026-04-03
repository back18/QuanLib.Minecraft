using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLib.Minecraft.Downloading
{
    public static class GameAssetHelper
    {
        public static AssetManifestTree BuildAssetManifestTree(string gameFolder, string gameVersion)
        {
            ArgumentException.ThrowIfNullOrEmpty(gameFolder, nameof(gameFolder));
            ArgumentException.ThrowIfNullOrEmpty(gameVersion, nameof(gameVersion));
            ThrowHelper.DirectoryNotFound(gameFolder);

            gameFolder = Path.GetFullPath(gameFolder);
            VersionJsonFile? versionJsonFile = QueryVersionJsonFile(gameFolder, gameVersion)
                ?? throw new InvalidOperationException($"Version '{gameVersion}' not found in any version json file.");

            string? assetFolder = GetAssetFolder(versionJsonFile.FilePath)
                ?? throw new InvalidOperationException($"Version json file '{versionJsonFile.FilePath}' is not in a valid version isolation folder.");

            string assetManifestId = versionJsonFile.VersionJson.Assets;
            string assetManifestFile = Path.Combine(assetFolder, "indexes", assetManifestId + ".json");
            AssetManifest assetManifest = LoadAssetManifest(assetManifestFile);

            string objectsFolder = Path.Combine(assetFolder, "objects");
            AssetObjectStorage assetObjectStorage = new(objectsFolder, assetManifest.Values.First().HashType);

            return new AssetManifestTree(assetObjectStorage, assetManifest);
        }

        public static async Task<AssetManifestTree> BuildAssetManifestTreeAsync(string gameFolder, string gameVersion)
        {
            ArgumentException.ThrowIfNullOrEmpty(gameFolder, nameof(gameFolder));
            ArgumentException.ThrowIfNullOrEmpty(gameVersion, nameof(gameVersion));
            ThrowHelper.DirectoryNotFound(gameFolder);

            gameFolder = Path.GetFullPath(gameFolder);
            VersionJsonFile? versionJsonFile = await QueryVersionJsonFileAsync(gameFolder, gameVersion)
                ?? throw new InvalidOperationException($"Version '{gameVersion}' not found in any version json file.");

            string? assetFolder = GetAssetFolder(versionJsonFile.FilePath)
                ?? throw new InvalidOperationException($"Version json file '{versionJsonFile.FilePath}' is not in a valid version isolation folder.");

            string assetManifestId = versionJsonFile.VersionJson.Assets;
            string assetManifestFile = Path.Combine(assetFolder, "indexes", assetManifestId + ".json");
            AssetManifest assetManifest = await LoadAssetManifestAsync(assetManifestFile);

            string objectsFolder = Path.Combine(assetFolder, "objects");
            AssetObjectStorage assetObjectStorage = new(objectsFolder, assetManifest.Values.First().HashType);

            return new AssetManifestTree(assetObjectStorage, assetManifest);
        }

        public static VersionJsonFile? QueryVersionJsonFile(string gameFolder, string gameVersion)
        {
            ArgumentException.ThrowIfNullOrEmpty(gameFolder, nameof(gameFolder));
            ArgumentException.ThrowIfNullOrEmpty(gameVersion, nameof(gameVersion));
            ThrowHelper.DirectoryNotFound(gameFolder);

            string[] versionJsonFiles = GetAllVersionJsonFile(gameFolder);
            foreach (string versionJsonFile in versionJsonFiles)
            {
                VersionJson? versionJson = TryLoadVersionJson(versionJsonFile);
                if (versionJson is null)
                    continue;

                VersionJsonPatch? versionPatch = versionJson.GetGamePatch();
                if (versionPatch is null)
                    continue;

                if (versionPatch.Version == gameVersion)
                    return new(versionJsonFile, versionJson);
            }

            return null;
        }

        public static async Task<VersionJsonFile?> QueryVersionJsonFileAsync(string gameFolder, string gameVersion)
        {
            ArgumentException.ThrowIfNullOrEmpty(gameFolder, nameof(gameFolder));
            ArgumentException.ThrowIfNullOrEmpty(gameVersion, nameof(gameVersion));
            ThrowHelper.DirectoryNotFound(gameFolder);

            string[] versionJsonFiles = GetAllVersionJsonFile(gameFolder);
            foreach (string versionJsonFile in versionJsonFiles)
            {
                VersionJson? versionJson = await TryLoadVersionJsonAsync(versionJsonFile);
                if (versionJson is null)
                    continue;

                VersionJsonPatch? versionPatch = versionJson.GetGamePatch();
                if (versionPatch is null)
                    continue;

                if (versionPatch.Version == gameVersion)
                    return new(versionJsonFile, versionJson);
            }

            return null;
        }

        public static string[] GetAllVersionJsonFile(string gameFolder)
        {
            ArgumentException.ThrowIfNullOrEmpty(gameFolder, nameof(gameFolder));

            List<string> versionJsonFiles = [];
            string? baseFolder = Path.GetDirectoryName(gameFolder);
            if (Directory.Exists(baseFolder) && Path.GetFileName(baseFolder) == "versions")
            {
                string jsonFile = Path.Combine(gameFolder, Path.GetFileName(gameFolder) + ".json");
                if (File.Exists(jsonFile))
                    versionJsonFiles.Add(jsonFile);
            }

            string versionIsolationFolder = Path.Combine(gameFolder, "versions");
            if (Directory.Exists(versionIsolationFolder))
            {
                string[] versionFolders = Directory.GetDirectories(versionIsolationFolder);
                foreach (string versionFolder in versionFolders)
                {
                    string jsonFile = Path.Combine(versionFolder, Path.GetFileName(versionFolder) + ".json");
                    if (File.Exists(jsonFile))
                        versionJsonFiles.Add(jsonFile);
                }
            }

            return versionJsonFiles.ToArray();
        }

        public static string? GetGameRootFolder(string versionJsonFile)
        {
            ArgumentException.ThrowIfNullOrEmpty(versionJsonFile, nameof(versionJsonFile));

            string? versionFolder = Path.GetDirectoryName(versionJsonFile);
            if (string.IsNullOrEmpty(versionFolder))
                return null;

            string? versionIsolationFolder = Path.GetDirectoryName(versionFolder);
            if (string.IsNullOrEmpty(versionIsolationFolder))
                return null;

            return Path.GetDirectoryName(versionIsolationFolder);
        }

        public static string? GetAssetFolder(string versionJsonFile)
        {
            ArgumentException.ThrowIfNullOrEmpty(versionJsonFile, nameof(versionJsonFile));

            string? gameFolder = GetGameRootFolder(versionJsonFile);
            if (string.IsNullOrEmpty(gameFolder))
                return null;

            return Path.Combine(gameFolder, "assets");
        }

        public static string? GetResourcePackFolder(string versionJsonFile)
        {
            ArgumentException.ThrowIfNullOrEmpty(versionJsonFile, nameof(versionJsonFile));

            string? baseFolder = Path.GetDirectoryName(versionJsonFile);
            if (string.IsNullOrEmpty(baseFolder))
                return null;

            string resourcePackFolder = Path.Combine(baseFolder, "resourcepacks");
            if (Directory.Exists(resourcePackFolder))
                return resourcePackFolder;

            string? gameFolder = GetGameRootFolder(versionJsonFile);
            if (string.IsNullOrEmpty(gameFolder))
                return null;

            resourcePackFolder = Path.Combine(gameFolder, "resourcepacks");
            if (Directory.Exists(resourcePackFolder))
                return resourcePackFolder;

            return null;
        }

        public static string? GetClientOptionsFile(string versionJsonFile)
        {
            ArgumentException.ThrowIfNullOrEmpty(versionJsonFile, nameof(versionJsonFile));

            string? versionFolder = Path.GetDirectoryName(versionJsonFile);
            if (string.IsNullOrEmpty(versionFolder))
                return null;

            string optionsFile = Path.Combine(versionFolder, "options.txt");
            if (File.Exists(optionsFile))
                return optionsFile;

            string? gameFolder = GetGameRootFolder(versionJsonFile);
            if (string.IsNullOrEmpty(gameFolder))
                return null;

            optionsFile = Path.Combine(gameFolder, "options.txt");
            if (File.Exists(optionsFile))
                return optionsFile;

            return null;
        }

        private static VersionJson? TryLoadVersionJson(string versionJsonFile)
        {
            try
            {
                string json = File.ReadAllText(versionJsonFile, Encoding.UTF8);
                JObject jobj = JObject.Parse(json);
                return new VersionJson(jobj);
            }
            catch
            {
                return null;
            }
        }

        private static async Task<VersionJson?> TryLoadVersionJsonAsync(string versionJsonFile)
        {
            try
            {
                string json = await File.ReadAllTextAsync(versionJsonFile, Encoding.UTF8);
                JObject jobj = JObject.Parse(json);
                return new VersionJson(jobj);
            }
            catch
            {
                return null;
            }
        }

        private static AssetManifest LoadAssetManifest(string assetManifestFile)
        {
            string json = File.ReadAllText(assetManifestFile, Encoding.UTF8);
            var model = JsonConvert.DeserializeObject<AssetManifest.Model>(json) ?? throw new InvalidDataException();
            return new AssetManifest(model);
        }

        private static async Task<AssetManifest> LoadAssetManifestAsync(string assetManifestFile)
        {
            string json = await File.ReadAllTextAsync(assetManifestFile, Encoding.UTF8);
            var model = JsonConvert.DeserializeObject<AssetManifest.Model>(json) ?? throw new InvalidDataException();
            return new AssetManifest(model);
        }

        public record VersionJsonFile(string FilePath, VersionJson VersionJson);
    }
}
