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
    public class LanguageManager : IReadOnlyDictionary<string, TemplateText>
    {
        internal LanguageManager(Dictionary<string, TemplateText> item, string language)
        {
            if (string.IsNullOrEmpty(language))
                throw new ArgumentException($"“{nameof(language)}”不能为 null 或空。", nameof(language));

            _items = item ?? throw new ArgumentNullException(nameof(item));
            Language = language;
        }

        private readonly Dictionary<string, TemplateText> _items;

        public TemplateText this[string key] => _items[key];

        public IEnumerable<string> Keys => _items.Keys;

        public IEnumerable<TemplateText> Values => _items.Values;

        public int Count => _items.Count;

        public string Language { get; }

        public bool ContainsKey(string key)
        {
            return _items.ContainsKey(key);
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out TemplateText value)
        {
            return _items.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<string, TemplateText>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_items).GetEnumerator();
        }
    }
}
