using Newtonsoft.Json.Linq;
using QuanLib.Core.Extension;
using QuanLib.Core.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.ResourcePack
{
    public class ResourceEntryManager : IReadOnlyDictionary<string, ResourceEntry>, IDisposable
    {
        public ResourceEntryManager()
        {
            _items = new();
            _models = new();
            _textures = new();
            ZipPacks = new();
        }

        private readonly Dictionary<string, ResourceEntry> _items;

        private readonly Dictionary<string, JObject> _models;

        private readonly Dictionary<string, Image<Rgba32>> _textures;

        public ResourceEntry this[string key] => _items[key];

        public IEnumerable<string> Keys => _items.Keys;

        public IEnumerable<ResourceEntry> Values => _items.Values;

        public int Count => _items.Count;

        internal List<ZipPack> ZipPacks { get; }

        public bool ContainsKey(string key)
        {
            return _items.ContainsKey(key);
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out ResourceEntry value)
        {
            return _items.TryGetValue(key, out value);
        }

        internal void Overwrite(ResourceEntry resourceEntry)
        {
            if (resourceEntry is null)
                throw new ArgumentNullException(nameof(resourceEntry));

            if (resourceEntry.IsEmpty)
                return;

            if (_items.TryGetValue(resourceEntry.ModID, out var result))
            {
                foreach (var blockState in resourceEntry.BlockStates)
                    result.BlockStates[blockState.Key] = blockState.Value;

                foreach (var blockModel in resourceEntry.BlockModels)
                    result.BlockModels[blockModel.Key] = blockModel.Value;

                foreach (var blockTexture in resourceEntry.BlockTextures)
                    result.BlockTextures[blockTexture.Key] = blockTexture.Value;

                foreach (var itemModel in resourceEntry.ItemModels)
                    result.ItemModels[itemModel.Key] = itemModel.Value;

                foreach (var itemTextures in resourceEntry.ItemTextures)
                    result.ItemTextures[itemTextures.Key] = itemTextures.Value;
            }
            else
            {
                _items.Add(resourceEntry.ModID, resourceEntry);
            }
        }

        public bool TryGetModel(string? modelID, [MaybeNullWhen(false)] out JObject result)
        {
            if (string.IsNullOrEmpty(modelID))
                goto err;

            if (_models.TryGetValue(modelID, out result))
                return true;

            string[] items1 = modelID.Split(':');
            if (items1.Length < 2)
                goto err;

            string[] items2 = items1[1].Split("/");
            if (items2.Length < 2)
                goto err;

            string modid = items1[0];
            string type = items2[0];
            string name = items2[1] + ".json";

            if (!_items.TryGetValue(modid, out var entry))
                goto err;

            ZipArchiveEntry? modelEntry = null;
            switch (type)
            {
                case "block":
                    entry.BlockModels.TryGetValue(name, out modelEntry);
                        break;
                case "item":
                    entry.ItemModels.TryGetValue(name, out modelEntry);
                    break;
            }

            if (modelEntry is null)
                goto err;

            try
            {
                using Stream stream = modelEntry.Open();
                result = JObject.Parse(stream.ToUtf8Text());
                _models.Add(modelID, result);
                return true;
            }
            catch
            {
                goto err;
            }

            err:
            result = null;
            return false;
        }

        public bool TryGetTexture(string? modelID, [MaybeNullWhen(false)] out Image<Rgba32> result)
        {
            if (string.IsNullOrEmpty(modelID))
                goto err;

            if (_textures.TryGetValue(modelID, out result))
                return true;

            string[] items1 = modelID.Split(':');
            if (items1.Length < 2)
                goto err;

            string[] items2 = items1[1].Split("/");
            if (items2.Length < 2)
                goto err;

            string modid = items1[0];
            string type = items2[0];
            string name = items2[1] + ".png";

            if (!_items.TryGetValue(modid, out var entry))
                goto err;

            ZipArchiveEntry? modelEntry = null;
            switch (type)
            {
                case "block":
                    entry.BlockTextures.TryGetValue(name, out modelEntry);
                    break;
                case "item":
                    entry.ItemTextures.TryGetValue(name, out modelEntry);
                    break;
            }

            if (modelEntry is null)
                goto err;

            try
            {
                using Stream stream = modelEntry.Open();
                result = Image.Load<Rgba32>(stream);
                _textures.Add(modelID, result);
                return true;
            }
            catch
            {
                goto err;
            }

            err:
            result = null;
            return false;
        }

        public IEnumerator<KeyValuePair<string, ResourceEntry>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_items).GetEnumerator();
        }

        public void Dispose()
        {
            foreach (var zipPack in ZipPacks)
                zipPack.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
