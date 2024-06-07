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
    public class VersionList : IReadOnlyDictionary<string, VersionIndex>
    {
        public VersionList(Model model)
        {
            NullValidator.ValidateObject(model, nameof(model));

            LatestVersion = new(model.latest);
            _items = model.versions.ToDictionary(item => item.id, versionModel => new VersionIndex(versionModel));
        }

        private readonly Dictionary<string, VersionIndex> _items;

        public VersionIndex this[string key] => _items[key];

        public IEnumerable<string> Keys => _items.Keys;

        public IEnumerable<VersionIndex> Values => _items.Values;

        public int Count => _items.Count;

        public LatestVersion LatestVersion { get; }

        public bool ContainsKey(string key)
        {
            return _items.ContainsKey(key);
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out VersionIndex value)
        {
            return _items.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<string, VersionIndex>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_items).GetEnumerator();
        }

        public class Model
        {
            public required LatestVersion.Model latest { get; set; }

            public required VersionIndex.Model[] versions { get; set; }
        }
    }
}
