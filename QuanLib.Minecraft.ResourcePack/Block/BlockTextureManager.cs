using QuanLib.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.ResourcePack.Block
{
    public class BlockTextureManager : IReadOnlyDictionary<string, BlockTexture>, ISingleton<BlockTextureManager, BlockTextureManager.InstantiateArgs>
    {
        internal BlockTextureManager(Dictionary<string, BlockTexture> items)
        {
            ArgumentNullException.ThrowIfNull(items, nameof(items));

            _items = items;
        }

        private static readonly object _slock = new();

        public static bool IsInstanceLoaded => _Instance is not null;

        public static BlockTextureManager Instance => _Instance ?? throw new InvalidOperationException("实例未加载");
        private static BlockTextureManager? _Instance;

        private readonly Dictionary<string, BlockTexture> _items;

        public BlockTexture this[string index] => _items[index];

        public IEnumerable<string> Keys => _items.Keys;

        public IEnumerable<BlockTexture> Values => _items.Values;

        public int Count => _items.Count;

        public static BlockTextureManager LoadInstance(InstantiateArgs instantiateArgs)
        {
            ArgumentNullException.ThrowIfNull(instantiateArgs, nameof(instantiateArgs));

            lock (_slock)
            {
                if (_Instance is not null)
                    throw new InvalidOperationException("试图重复加载单例实例");

                _Instance = BlockTextureReader.Load(instantiateArgs.ResourceEntryManager, instantiateArgs.Blacklist);
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

        public class InstantiateArgs : Core.InstantiateArgs
        {
            public InstantiateArgs(ResourceEntryManager resourceEntryManager, IEnumerable<BlockState> blacklist)
            {
                ArgumentNullException.ThrowIfNull(resourceEntryManager, nameof(resourceEntryManager));
                ArgumentNullException.ThrowIfNull(blacklist, nameof(blacklist));

                ResourceEntryManager = resourceEntryManager;
                Blacklist = blacklist;
            }

            public ResourceEntryManager ResourceEntryManager { get; }

            public IEnumerable<BlockState> Blacklist { get; }
        }
    }
}
