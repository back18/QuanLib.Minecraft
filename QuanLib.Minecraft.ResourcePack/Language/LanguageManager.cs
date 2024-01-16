using QuanLib.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.ResourcePack.Language
{
    public class LanguageManager : IReadOnlyDictionary<string, TextTemplate>, ISingleton<LanguageManager, LanguageManager.InstantiateArgs>
    {
        internal LanguageManager(Dictionary<string, TextTemplate> item, string language)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));
            ArgumentException.ThrowIfNullOrEmpty(language, nameof(language));

            _items = item;
            Language = language;
        }

        private static readonly object _slock = new();

        public static bool IsInstanceLoaded => _Instance is not null;

        public static LanguageManager Instance => _Instance ?? throw new InvalidOperationException("实例未加载");
        private static LanguageManager? _Instance;

        private readonly Dictionary<string, TextTemplate> _items;

        public TextTemplate this[string key] => _items[key];

        public IEnumerable<string> Keys => _items.Keys;

        public IEnumerable<TextTemplate> Values => _items.Values;

        public int Count => _items.Count;

        public string Language { get; }

        public static LanguageManager LoadInstance(InstantiateArgs instantiateArgs)
        {
            ArgumentNullException.ThrowIfNull(instantiateArgs, nameof(instantiateArgs));

            lock (_slock)
            {
                if (_Instance is not null)
                    throw new InvalidOperationException("试图重复加载单例实例");

                _Instance = LanguageReader.Load(instantiateArgs.ResourceEntryManager, instantiateArgs.Language);
                return _Instance;
            }
        }

        public bool ContainsKey(string key)
        {
            return _items.ContainsKey(key);
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out TextTemplate value)
        {
            return _items.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<string, TextTemplate>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_items).GetEnumerator();
        }

        public class InstantiateArgs : Core.InstantiateArgs
        {
            public InstantiateArgs(ResourceEntryManager resourceEntryManager, string language)
            {
                ArgumentNullException.ThrowIfNull(resourceEntryManager, nameof(resourceEntryManager));
                ArgumentException.ThrowIfNullOrEmpty(language, nameof(language));

                ResourceEntryManager = resourceEntryManager;
                Language = language;
            }

            public ResourceEntryManager ResourceEntryManager { get; }

            public string Language { get; }
        }
    }
}
