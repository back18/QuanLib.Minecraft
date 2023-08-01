using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public class CursorManager : IReadOnlyDictionary<string, Cursor>
    {
        private CursorManager(Dictionary<string, Cursor> items)
        {
            _items = items;
        }

        private readonly Dictionary<string, Cursor> _items;

        public Cursor this[string key] => _items[key];

        public IEnumerable<string> Keys => _items.Keys;

        public IEnumerable<Cursor> Values => _items.Values;

        public int Count => _items.Count;

        public static CursorManager Load(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException($"“{nameof(path)}”不能为 null 或空。", nameof(path));

            Dictionary<string, Cursor> result = new();
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                Cursor cursor = new(JsonConvert.DeserializeObject<Cursor.Json>(File.ReadAllText(file)) ?? throw new FormatException());
                result.Add(cursor.CursorType, cursor);
            }
            return new(result);
        }

        public bool ContainsKey(string key)
        {
            return _items.ContainsKey(key);
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out Cursor value)
        {
            return _items.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<string, Cursor>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_items).GetEnumerator();
        }
    }
}
