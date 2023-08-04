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
            FormList = new(this);

            AddedForm += OnAddedForm;
            RemovedForm += OnAddedForm;
        }

        public FormCollection FormList { get; }

        public event EventHandler<FormManager, FormContextEventArgs> AddedForm;

        public event EventHandler<FormManager, FormContextEventArgs> RemovedForm;

        protected virtual void OnAddedForm(FormManager sender, FormContextEventArgs e) { }

        protected virtual void OnRemovedForm(FormManager sender, FormContextEventArgs e) { }

        public class FormCollection : IList<FormContext>, IReadOnlyList<FormContext>
        {
            public FormCollection(FormManager owner)
            {
                _owner = owner ?? throw new ArgumentNullException(nameof(owner));
                _items = new();
            }

            private readonly FormManager _owner;

            private readonly List<FormContext> _items;

            public FormContext this[int index] => _items[index];

            FormContext IList<FormContext>.this[int index] { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

            public int Count => _items.Count;

            public bool IsReadOnly => false;

            public void Add(FormContext item)
            {
                if (item is null)
                    throw new ArgumentNullException(nameof(item));

                _items.Add(item);
                _owner.AddedForm.Invoke(_owner, new(item));
            }

            public bool TryAdd(FormContext item)
            {
                if (_items.Contains(item))
                    return false;

                Add(item);
                return true;
            }

            public bool Remove(FormContext item)
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

            public bool Contains(FormContext item)
            {
                return _items.Contains(item);
            }

            public int IndexOf(FormContext item)
            {
                return _items.IndexOf(item);
            }

            public void CopyTo(FormContext[] array, int arrayIndex)
            {
                _items.CopyTo(array, arrayIndex);
            }

            public FormContext[] ToArray()
            {
                return _items.ToArray();
            }

            public IEnumerator<FormContext> GetEnumerator()
            {
                return _items.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)_items).GetEnumerator();
            }

            void IList<FormContext>.Insert(int index, FormContext item)
            {
                throw new NotSupportedException();
            }
        }
    }
}
