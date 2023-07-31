using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public class FormManager
    {
        public FormManager()
        {
            Forms = new(this);

            AddedForm += OnAddedForm;
            RemovedForm += OnAddedForm;
        }

        public FormCollection Forms { get; }

        public event EventHandler<FormManager, FormEventArgs> AddedForm;

        public event EventHandler<FormManager, FormEventArgs> RemovedForm;

        protected virtual void OnAddedForm(FormManager sender, FormEventArgs e) { }

        protected virtual void OnRemovedForm(FormManager sender, FormEventArgs e) { }

        public class FormCollection : IList<IForm>, IReadOnlyList<IForm>
        {
            public FormCollection(FormManager owner)
            {
                _owner = owner ?? throw new ArgumentNullException(nameof(owner));
                _items = new();
            }

            private readonly FormManager _owner;

            private readonly List<IForm> _items;

            public IForm this[int index] => _items[index];

            IForm IList<IForm>.this[int index] { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

            public int Count => _items.Count;

            public bool IsReadOnly => false;

            public void Add(IForm item)
            {
                if (item is null)
                    throw new ArgumentNullException(nameof(item));

                _items.Add(item);
                _owner.AddedForm.Invoke(_owner, new(item));
            }

            public bool TryAdd(IForm item)
            {
                if (_items.Contains(item))
                    return false;

                Add(item);
                return true;
            }

            public bool Remove(IForm item)
            {
                if (item is null)
                    throw new ArgumentNullException(nameof(item));

                if (!_items.Remove(item))
                    return false;

                _owner.RemovedForm.Invoke(_owner, new(item));
                return true;
            }

            public void RemoveAt(int index)
            {
                Remove(_items[index]);
            }

            public void Clear()
            {
                foreach (var item in _items.ToArray())
                    Remove(item);
            }

            public bool Contains(IForm item)
            {
                return _items.Contains(item);
            }

            public int IndexOf(IForm item)
            {
                return _items.IndexOf(item);
            }

            public void CopyTo(IForm[] array, int arrayIndex)
            {
                _items.CopyTo(array, arrayIndex);
            }

            public IForm[] ToArray()
            {
                return _items.ToArray();
            }

            public IEnumerator<IForm> GetEnumerator()
            {
                return _items.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)_items).GetEnumerator();
            }

            void IList<IForm>.Insert(int index, IForm item)
            {
                throw new NotSupportedException();
            }
        }
    }
}
