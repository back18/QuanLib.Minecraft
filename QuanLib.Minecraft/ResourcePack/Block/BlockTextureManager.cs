using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.ResourcePack.Block
{
    public class BlockTextureManager : IReadOnlyDictionary<string, BlockTexture>
    {
        static BlockTextureManager()
        {
            _slock = new();
            IsLoaded = false;
        }

        internal BlockTextureManager(Dictionary<string, BlockTexture> items)
        {
            ArgumentNullException.ThrowIfNull(items, nameof(items));

            _items = items;
        }

        private static readonly object _slock;

        public static bool IsLoaded { get; private set; }

        public static BlockTextureManager Instance
        {
            get
            {
                if (_Instance is null)
                    throw new InvalidOperationException("实例未加载");
                return _Instance;
            }
        }
        private static BlockTextureManager? _Instance;

        private readonly Dictionary<string, BlockTexture> _items;

        public BlockTexture this[string index] => _items[index];

        public IEnumerable<string> Keys => _items.Keys;

        public IEnumerable<BlockTexture> Values => _items.Values;

        public int Count => _items.Count;

        public static BlockTextureManager LoadInstance(ResourceEntryManager resources, IEnumerable<BlockState> blacklist)
        {
            ArgumentNullException.ThrowIfNull(resources, nameof(resources));
            ArgumentNullException.ThrowIfNull(blacklist, nameof(blacklist));

            lock (_slock)
            {
                if (_Instance is not null)
                    throw new InvalidOperationException("试图重复加载单例实例");

                _Instance = BlockTextureReader.Load(resources, blacklist);
                IsLoaded = true;
                return _Instance;
            }
        }

        public bool ContainsKey(string key)
        {
            return _items.ContainsKey(key);
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out BlockTexture value)
        {
            if (_items.TryGetValue(key, out value))
                return true;

            foreach (var item in _items)
            {
                if (item.Key.StartsWith(key))
                {
                    value = item.Value;
                    return true;
                }
            }

            return false;
        }

        public IEnumerator<KeyValuePair<string, BlockTexture>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_items).GetEnumerator();
        }
    }
}
