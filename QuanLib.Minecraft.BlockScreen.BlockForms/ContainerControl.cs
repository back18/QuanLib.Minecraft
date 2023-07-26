using QuanLib.Minecraft.BlockScreen.UI;
using SixLabors.ImageSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public abstract class ContainerControl<TControl> : ContainerControl where TControl : Control
    {
        protected ContainerControl()
        {
            SubControls = new(this);
        }

        public ControlCollection<TControl> SubControls { get; }

        public override IReadOnlyControlCollection<Control> GetSubControls() => SubControls;

        public override ControlCollection<T>? AsControlCollection<T>()
        {
            if (SubControls is ControlCollection<T> result)
                return result;

            return null;
        }

        public override void ClearAllLayoutSyncer()
        {
            foreach (Control control in SubControls)
            {
                control.ClearAllLayoutSyncer();
            }

            base.ClearAllLayoutSyncer();
        }
    }

    public abstract class ContainerControl : AbstractContainer<Control>
    {
        protected ContainerControl()
        {
            OnAddedSubControl += (obj) => { };
            OnRemovedSubControl += (obj) => { };
        }

        public abstract ControlCollection<T>? AsControlCollection<T>() where T : Control;

        public override event Action<Control> OnAddedSubControl;

        public override event Action<Control> OnRemovedSubControl;

        public virtual void ActiveLayoutAll()
        {

        }

        public override void LayoutAll(Size oldSize, Size newSize)
        {
            foreach (var control in GetSubControls())
            {
                if (control.LayoutMode == LayoutMode.Auto)
                    control.Layout(oldSize, newSize);
            }
        }

        public override void UpdateAllHoverState(Point position, CursorMode mode)
        {
            foreach (var control in GetSubControls().ToArray())
            {
                control.UpdateAllHoverState(control.ParentPos2SubPos(position), mode);
            }

            base.UpdateAllHoverState(position, mode);
        }

        public class ControlCollection<T> : AbstractControlCollection<T> where T : Control
        {
            public ControlCollection(ContainerControl owner)
            {
                _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            }

            private readonly ContainerControl _owner;

            public override void Add(T item)
            {
                if (item is null)
                    throw new ArgumentNullException(nameof(item));

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

                ((IControl)item).SetGenericContainerControl(_owner);
                RecentlyAddedControl = item;
                _owner.OnAddedSubControl.Invoke(item);
                _owner.RequestUpdateFrame();
            }

            public override bool Remove(T item)
            {
                if (item is null)
                    throw new ArgumentNullException(nameof(item));

                if (!_items.Remove(item))
                    return false;

                ((IControl)item).SetGenericContainerControl(null);
                RecentlyRemovedControl = item;
                _owner.OnAddedSubControl.Invoke(item);
                _owner.RequestUpdateFrame();
                return true;
            }

            public void ClearSelected()
            {
                foreach (T control in _items.ToArray())
                    control.IsSelected = false;
            }

            public void ClearSyncers()
            {
                foreach (T control in _items)
                    control.LayoutSyncer = null;
            }
        }
    }
}
