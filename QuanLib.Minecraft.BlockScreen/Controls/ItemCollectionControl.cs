using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Controls
{
    public abstract class ItemCollectionControl<T> : Control where T : notnull
    {
        protected ItemCollectionControl()
        {
            Items = new(this);
            _SelectedItemIndex = -1;
            ItemToString = (item) =>
            {
                if (item is null)
                    return string.Empty;

                if (Items._names.TryGetValue(item, out var name))
                    return name;
                else
                    return item.ToString();
            };
            SelectedItemIndexChanged += (arg1, arg2) => { };
            SelectedItemChanged += (arg1, arg2) => { };
            BeforeFrame += ItemCollectionControl_BeforeFrame;

            SelectedItem_Update = false;
            SelectedItem_Old = SelectedItem;
        }

        private bool SelectedItem_Update;

        private T? SelectedItem_Old;

        public ItemCollection Items { get; }

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
                    else if (value > Items.Count - 1)
                        value = Items.Count - 1;
                    _SelectedItemIndex = value;
                    SelectedItemIndexChanged.Invoke(temp, _SelectedItemIndex);
                    if (!Equals(SelectedItem, SelectedItem_Old))
                        SelectedItem_Update = true;
                    RequestUpdateFrame();
                }
            }
        }
        private int _SelectedItemIndex;

        public T? SelectedItem
        {
            get => SelectedItemIndex < 0 ? default : Items[SelectedItemIndex];
            set => SelectedItemIndex = value is null ? default : Items.IndexOf(value);
        }

        public Func<T?, string?> ItemToString { get; set; }

        public event Action<int, int> SelectedItemIndexChanged;

        public event Action<T?, T?> SelectedItemChanged;

        private void ItemCollectionControl_BeforeFrame()
        {
            if (SelectedItem_Update)
            {
                if (!Equals(SelectedItem, SelectedItem_Old))
                    SelectedItemChanged.Invoke(SelectedItem_Old, SelectedItem);
                SelectedItem_Update = false;
                SelectedItem_Old = SelectedItem;
            }
        }

        public override void OnInitComplete3()
        {
            SelectedItem_Update = false;
            SelectedItem_Old = SelectedItem;
            SelectedItemChanged.Invoke(SelectedItem, SelectedItem);

            base.OnInitComplete3();
        }

        public class ItemCollection : IList<T>, IReadOnlyList<T>
        {
            public ItemCollection(ItemCollectionControl<T> owner)
            {
                _owner = owner;
                _items = new();
                _names = new();
            }

            private readonly ItemCollectionControl<T> _owner;

            private readonly List<T> _items;

            internal readonly Dictionary<T, string> _names;

            public T this[int index] { get => _items[index]; set => _items[index] = value; }

            public int Count => _items.Count;

            public bool IsReadOnly => false;

            public void Add(T item)
            {
                _items.Add(item);

                if (_owner.SelectedItemIndex < 0)
                    _owner.SelectedItemIndex = 0;
            }

            public void Add(T item, string name)
            {
                Add(item);
                _names[item] = name;
            }

            public void AddRenge(IEnumerable<T> collection)
            {
                _items.AddRange(collection);

                if (_owner.SelectedItemIndex < 0)
                    _owner.SelectedItemIndex = 0;
            }

            public void Insert(int index, T item)
            {
                _items.Insert(index, item);

                if (_owner.SelectedItemIndex < 0)
                    _owner.SelectedItemIndex = 0;
                else if (_owner.SelectedItemIndex >= index)
                    _owner.SelectedItemIndex++;
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

                if (result && _owner.SelectedItemIndex >= index)
                    _owner.SelectedItemIndex--;

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
        }
    }
}
