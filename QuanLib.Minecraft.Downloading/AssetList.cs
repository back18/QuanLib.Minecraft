using Newtonsoft.Json;
using QuanLib.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Downloading
{
    public class AssetList : IReadOnlyDictionary<string, AssetIndex>
    {
        public AssetList(Model model)
        {
            NullValidator.ValidateObject(model, nameof(model));

            _items = new();
            _items = model.objects.ToDictionary(item => item.Key, item => new AssetIndex(item.Value));
        }

        private readonly Dictionary<string, AssetIndex> _items;

        public IEnumerable<string> Keys => _items.Keys;

        public IEnumerable<AssetIndex> Values => _items.Values;

        public int Count => _items.Count;

        public AssetIndex this[string key] => _items[key];

        public bool ContainsKey(string key)
        {
            return _items.ContainsKey(key);
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out AssetIndex value)
        {
            return _items.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<string, AssetIndex>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_items).GetEnumerator();
        }

        public class Model
        {
            public required Dictionary<string, AssetIndex.Model> objects { get; set; }
        }
    }
}
