using QuanLib.Event;
using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.UI
{
    public class ItemCollection<T> : IList<T>, IReadOnlyList<T> where T : notnull
    {
        public ItemCollection()
        {
            _items = new();
            _names = new();

            _SelectedItemIndex = -1;
            SelectedItem_Old = default;
            ItemToStringFunc = (item) =>
            {
                if (item is null)
                    return string.Empty;

                if (_names.TryGetValue(item, out var name))
                    return name;
                else
                    return item.ToString() ?? string.Empty;
            };

            SelectedItemIndexChanged += OnSelectedItemIndexChanged;
            SelectedItemChanged += OnSelectedItemChanged;
        }

        private readonly List<T> _items;

        internal readonly Dictionary<T, string> _names;

        private T? SelectedItem_Old;

        public T this[int index] { get => _items[index]; set => _items[index] = value; }

        public int Count => _items.Count;

        public bool IsReadOnly => false;

        public int SelectedItemIndex
        {
            get => _SelectedItemIndex;
            set
            {
                if (_SelectedItemIndex != value)
                {
                    int temp = _SelectedItemIndex;
                    if (value < 0)
                        value = 0;
                    else if (value > _items.Count - 1)
                        value = _items.Count - 1;
                    _SelectedItemIndex = value;
                    SelectedItemIndexChanged.Invoke(this, new(temp, _SelectedItemIndex));
                    if (!Equals(SelectedItem, SelectedItem_Old))
                    {
                        SelectedItemChanged.Invoke(this, new(SelectedItem_Old, SelectedItem));
                        SelectedItem_Old = SelectedItem;
                    }
                }
            }
        }
        private int _SelectedItemIndex;

        public T? SelectedItem
        {
            get => SelectedItemIndex < 0 ? default : _items[SelectedItemIndex];
            set => SelectedItemIndex = value is null ? default : _items.IndexOf(value);
        }

        public Func<T?, string> ItemToStringFunc { get; set; }

        public event EventHandler<ItemCollection<T>, IndexChangedEventArgs> SelectedItemIndexChanged;

        public event EventHandler<ItemCollection<T>, ValueChangedEventArgs<T?>> SelectedItemChanged;

        protected virtual void OnSelectedItemIndexChanged(ItemCollection<T> sender, IndexChangedEventArgs e) { }

        protected virtual void OnSelectedItemChanged(ItemCollection<T> sender, ValueChangedEventArgs<T?> e) { }

        public void Add(T item)
        {
            _items.Add(item);

            if (SelectedItemIndex < 0)
                SelectedItemIndex = 0;
        }

        public void Add(T item, string name)
        {
            Add(item);
            _names[item] = name;
        }

        public void AddRenge(IEnumerable<T> collection)
        {
            _items.AddRange(collection);

            if (SelectedItemIndex < 0)
                SelectedItemIndex = 0;
        }

        public void Insert(int index, T item)
        {
            _items.Insert(index, item);

            if (SelectedItemIndex < 0)
                SelectedItemIndex = 0;
            else if (SelectedItemIndex >= index)
                SelectedItemIndex++;
        }

        public void Insert(int index, T item, string name)
        {
            Insert(index, item);
            _names[item] = name;
        }

        public bool Remove(T item)
        {
            int index = _items.IndexOf(item);
            bool result = _items.Remove(item);

            if (_names.ContainsKey(item))
                _names.Remove(item);

            if (result && SelectedItemIndex >= index)
                SelectedItemIndex--;

            return result;
        }

        public void RemoveAt(int index)
        {
            Remove(_items[index]);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        public int IndexOf(T item)
        {
            return _items.IndexOf(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_items).GetEnumerator();
        }

        public string ItemToString(T? item) => ItemToStringFunc(item);
    }
}
