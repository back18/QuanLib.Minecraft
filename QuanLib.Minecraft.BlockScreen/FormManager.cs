using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.BlockScreen.Screens;
using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public class FormManager
    {
        public FormManager()
        {
            Items = new(this);

            AddedForm += OnAddedForm;
            RemovedForm += OnAddedForm;
        }

        public FormCollection Items { get; }

        public event EventHandler<FormManager, FormContextEventArgs> AddedForm;

        public event EventHandler<FormManager, FormContextEventArgs> RemovedForm;

        protected virtual void OnAddedForm(FormManager sender, FormContextEventArgs e) { }

        protected virtual void OnRemovedForm(FormManager sender, FormContextEventArgs e) { }

        public void FormScheduling()
        {
            foreach (var context in Items)
            {
                context.Value.Handle();
                if (context.Value.FormState == FormState.Closed)
                    Items.Remove(context.Key);
            }
        }

        public class FormCollection : IDictionary<int, FormContext>
        {
            public FormCollection(FormManager owner)
            {
                _owner = owner ?? throw new ArgumentNullException(nameof(owner));
                _items = new();
            }

            private readonly FormManager _owner;

            private readonly ConcurrentDictionary<int, FormContext> _items;

            private int _id;

            public ICollection<int> Keys => _items.Keys;

            public ICollection<FormContext> Values => _items.Values;

            public int Count => _items.Count;

            public bool IsReadOnly => false;

            public FormContext this[int index] => _items[index];

            FormContext IDictionary<int, FormContext>.this[int index] { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

            public FormContext Add(Application application, IForm form)
            {
                if (application is null)
                    throw new ArgumentNullException(nameof(application));
                if (form is null)
                    throw new ArgumentNullException(nameof(form));

                lock (this)
                {
                    int id = _id;
                    FormContext context = new(application, form);
                    context.ID = id;
                    _items.TryAdd(id, context);
                    _owner.AddedForm.Invoke(_owner, new(context));
                    _id++;
                    return context;
                }
            }

            public bool Remove(int id)
            {
                lock (this)
                {
                    if (!_items.TryGetValue(id, out var context) || !_items.TryRemove(id, out _))
                        return false;

                    context.ID = -1;
                    _owner.RemovedForm.Invoke(_owner, new(context));
                    return true;
                }
            }

            public void Clear()
            {
                foreach (var id in _items.Keys.ToArray())
                    Remove(id);
            }

            public bool ContainsKey(int id)
            {
                return _items.ContainsKey(id);
            }

            public bool TryGetValue(int id, [MaybeNullWhen(false)] out FormContext context)
            {
                return _items.TryGetValue(id, out context);
            }

            public IEnumerator<KeyValuePair<int, FormContext>> GetEnumerator()
            {
                return _items.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)_items).GetEnumerator();
            }

            void ICollection<KeyValuePair<int, FormContext>>.Add(KeyValuePair<int, FormContext> item)
            {
                ((ICollection<KeyValuePair<int, FormContext>>)_items).Add(item);
            }

            bool ICollection<KeyValuePair<int, FormContext>>.Remove(KeyValuePair<int, FormContext> item)
            {
                return ((ICollection<KeyValuePair<int, FormContext>>)_items).Remove(item);
            }

            bool ICollection<KeyValuePair<int, FormContext>>.Contains(KeyValuePair<int, FormContext> item)
            {
                return ((ICollection<KeyValuePair<int, FormContext>>)_items).Contains(item);
            }

            void ICollection<KeyValuePair<int, FormContext>>.CopyTo(KeyValuePair<int, FormContext>[] array, int arrayIndex)
            {
                ((ICollection<KeyValuePair<int, FormContext>>)_items).CopyTo(array, arrayIndex);
            }

            void IDictionary<int, FormContext>.Add(int key, FormContext value)
            {
                throw new NotSupportedException();
            }
        }
    }
}
