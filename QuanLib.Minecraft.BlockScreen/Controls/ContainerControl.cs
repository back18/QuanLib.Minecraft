using SixLabors.ImageSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Controls
{
    public abstract class ContainerControl<T> : ContainerControl where T : Control
    {
        protected ContainerControl()
        {
            SubControls = new(this);
        }

        public override IReadOnlyControlCollection<Control> GetSubControls() => SubControls;

        public override ControlCollection<R>? AsControlCollection<R>()
        {
            if (SubControls is ControlCollection<R> result)
                return result;

            return null;
        }

        public ControlCollection<T> SubControls { get; }

        public Type SubControlType => typeof(T);

        public bool IsSubControlType<R>() => typeof(R) == SubControlType;
    }

    public abstract class ContainerControl : Control
    {
        protected ContainerControl()
        {
            OnAddSubControl += (obj) => { };
            OnRemoveSubControl += (obj) => { };
            OnLayoutSubControl += LayoutAll;

            OnResize += ControlContainer_OnResize;
        }

        public abstract IReadOnlyControlCollection<Control> GetSubControls();

        public abstract ControlCollection<R>? AsControlCollection<R>() where R : Control;

        public event Action<Control> OnAddSubControl;

        public event Action<Control> OnRemoveSubControl;

        public event Action<Size, Size> OnLayoutSubControl;

        private void ControlContainer_OnResize(Size oldSize, Size newSize)
        {
            OnLayoutSubControl.Invoke(oldSize, newSize);
        }

        public virtual void ActiveLayoutAll()
        {

        }

        public virtual void LayoutAll(Size oldSize, Size newSize)
        {
            foreach (var control in GetSubControls())
            {
                if (control.LayoutMode == LayoutMode.Auto)
                    control.Layout(oldSize, newSize);
            }
        }

        internal override void HandleCursorMove(Point position, CursorMode mode)
        {
            foreach (var control in GetSubControls().ToArray())
            {
                control.HandleCursorMove(control.ParentPos2SubPos(position), mode);
            }

            base.HandleCursorMove(position, mode);
        }

        internal override bool HandleRightClick(Point position)
        {
            Control? control = GetSubControls().FirstHover;
            control?.HandleRightClick(control.ParentPos2SubPos(position));

            return base.HandleRightClick(position);
        }

        internal override bool HandleLeftClick(Point position)
        {
            Control? control = GetSubControls().FirstHover;
            control?.HandleLeftClick(control.ParentPos2SubPos(position));

            return base.HandleLeftClick(position);
        }

        internal override void HandleTextEditorUpdate(Point position, string text)
        {
            foreach (var control in GetSubControls().ToArray())
            {
                control.HandleTextEditorUpdate(control.ParentPos2SubPos(position), text);
            }

            base.HandleTextEditorUpdate(position, text);
        }

        internal override void HandleBeforeFrame()
        {
            foreach (var control in GetSubControls().ToArray())
            {
                control.HandleBeforeFrame();
            }

            base.HandleBeforeFrame();
        }

        internal override void HandleAfterFrame()
        {
            foreach (var control in GetSubControls().ToArray())
            {
                control.HandleAfterFrame();
            }

            base.HandleAfterFrame();
        }

        public class ControlCollection<T> : IList<T>, IReadOnlyControlCollection<T> where T : Control
        {
            public ControlCollection(ContainerControl owner)
            {
                _owner = owner ?? throw new ArgumentNullException(nameof(owner));
                _items = new();
            }

            private readonly ContainerControl _owner;

            private readonly List<T> _items;

            public int Count => _items.Count;

            public bool IsReadOnly => false;

            public bool HaveHover => FirstHover is not null;

            public bool HaveSelected => FirstSelected is not null;

            public T? FirstHover
            {
                get
                {
                    for (int i = _items.Count - 1; i >= 0; i--)
                    {
                        if (_items[i].IsHover)
                            return _items[i];
                    }
                    return null;
                }
            }

            public T? FirstSelected
            {
                get
                {
                    for (int i = _items.Count - 1; i >= 0; i--)
                    {
                        if (_items[i].IsSelected)
                            return _items[i];
                    }
                    return null;
                }
            }

            public T? RecentlyAddedControl { get; private set; }

            public T? RecentlyRemovedControl { get; private set; }

            public T this[int index] => _items[index];

            T IList<T>.this[int index] { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

            public void Add(T item)
            {
                if (item is null)
                    throw new ArgumentNullException(nameof(item));

                item.ParentContainer = _owner;
                item.OnSelected += Sort;
                item.OnDeselected += Sort;

                bool insert = false;
                for (int i = _items.Count - 1; i >= 0; i--)
                {
                    if (item.DisplayPriority >= _items[i].DisplayPriority)
                    {
                        _items.Insert(i + 1, item);
                        insert = true;
                        break;
                    }
                }
                if (!insert)
                    _items.Insert(0, item);
                RecentlyAddedControl = item;
                _owner.OnAddSubControl.Invoke(item);
                _owner.RequestUpdateFrame();
            }

            public bool TryAdd(T item)
            {
                if (_items.Contains(item))
                {
                    return false;
                }
                else
                {
                    Add(item);
                    return true;
                }
            }

            public bool Remove(T item)
            {
                if (item is null)
                    throw new ArgumentNullException(nameof(item));

                item.ParentContainer = null;
                item.OnSelected -= Sort;
                item.OnDeselected -= Sort;
                bool result = _items.Remove(item);
                RecentlyRemovedControl = item;
                _owner.OnAddSubControl.Invoke(item);
                _owner.RequestUpdateFrame();

                return result;
            }

            public void RemoveAt(int index)
            {
                _items.Remove(_items[index]);
            }

            public void Clear()
            {
                foreach (var item in _items.ToArray())
                    Remove(item);
            }

            public bool Contains(T item)
            {
                return _items.Contains(item);
            }

            public int IndexOf(T item)
            {
                return _items.IndexOf(item);
            }

            public IReadOnlyList<T> GetHovers()
            {
                List<T> result = new();
                foreach (var item in _items)
                    if (item.IsHover)
                        result.Add(item);
                return result;
            }

            public IReadOnlyList<T> GetSelecteds()
            {
                List<T> result = new();
                foreach (var item in _items)
                    if (item.IsSelected)
                        result.Add(item);
                return result;
            }

            public void ClearSelected()
            {
                foreach (T control in _items.ToArray())
                    control.IsSelected = false;
            }

            public void ClearSyncers()
            {
                foreach (T control in _items)
                    control.ControlSyncer = null;
            }

            public void Sort()
            {
                _items.Sort();
            }

            public T[] ToArray()
            {
                return _items.ToArray();
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

            void IList<T>.Insert(int index, T item)
            {
                throw new NotSupportedException();
            }
        }
    }
}
