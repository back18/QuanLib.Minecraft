using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.ResourcePack.Language
{
    public class LanguageManager : IReadOnlyDictionary<string, TextTemplate>
    {
        static LanguageManager()
        {
            _slock = new();
            IsLoaded = false;
        }

        internal LanguageManager(Dictionary<string, TextTemplate> item, string language)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));
            ArgumentException.ThrowIfNullOrEmpty(language, nameof(language));

            _items = item;
            Language = language;
        }

        private static readonly object _slock;

        public static bool IsLoaded { get; private set; }

        public static LanguageManager Instance
        {
            get
            {
                if (_Instance is null)
                    throw new InvalidOperationException("实例未加载");
                return _Instance;
            }
        }
        private static LanguageManager? _Instance;

        private readonly Dictionary<string, TextTemplate> _items;

        public TextTemplate this[string key] => _items[key];

        public IEnumerable<string> Keys => _items.Keys;

        public IEnumerable<TextTemplate> Values => _items.Values;

        public int Count => _items.Count;

        public string Language { get; }

        public static LanguageManager LoadInstance(ResourceEntryManager resources, string language)
        {
            ArgumentNullException.ThrowIfNull(resources, nameof(resources));
            ArgumentException.ThrowIfNullOrEmpty(language, nameof(language));

            lock (_slock)
            {
                if (_Instance is not null)
                    throw new InvalidOperationException("试图重复加载单例实例");

                _Instance = LanguageReader.Load(resources, language);
                IsLoaded = true;
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
    }
}
