using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public abstract class GenericContainerControl<TControl> : GenericContainerControl where TControl : class, IControl
    {
        protected GenericContainerControl()
        {
            SubControls = new(this);
        }

        public ControlCollection<TControl> SubControls { get; }

        public override IReadOnlyControlCollection<IControl> GetSubControls() => SubControls;

        public override ControlCollection<T>? AsControlCollection<T>()
        {
            if (SubControls is ControlCollection<T> result)
                return result;

            return null;
        }
    }

    public abstract class GenericContainerControl : AbstractContainer<IControl>
    {
        protected GenericContainerControl()
        {
            AddedSubControl += (sender, e) => { };
            RemovedSubControl += (sender, e) => { };
        }

        public abstract ControlCollection<T>? AsControlCollection<T>() where T : class, IControl;

        public override event EventHandler<AbstractContainer<IControl>, ControlEventArgs<IControl>> AddedSubControl;

        public override event EventHandler<AbstractContainer<IControl>, ControlEventArgs<IControl>> RemovedSubControl;

        public class ControlCollection<T> : AbstractControlCollection<T> where T : class, IControl
        {
            public ControlCollection(GenericContainerControl owner)
            {
                _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            }

            private readonly GenericContainerControl _owner;

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

                item.SetGenericContainerControl(_owner);
                RecentlyAddedControl = item;
                _owner.AddedSubControl.Invoke(_owner, new(item));
                _owner.RequestUpdateFrame();
            }

            public override bool Remove(T item)
            {
                if (item is null)
                    throw new ArgumentNullException(nameof(item));

                if (!_items.Remove(item))
                    return false;

                item.SetGenericContainerControl(null);
                RecentlyRemovedControl = item;
                _owner.RemovedSubControl.Invoke(_owner, new(item));
                _owner.RequestUpdateFrame();
                return true;
            }
        }
    }
}
