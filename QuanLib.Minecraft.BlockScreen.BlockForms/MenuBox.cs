using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using QuanLib.Minecraft.BlockScreen.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public abstract class MenuBox<T> : ScrollablePanel where T : Control
    {
        protected MenuBox()
        {
            Spacing = 1;
            _items = new();
        }

        protected List<T> _items;

        protected T? PreviousSubControl
        {
            get
            {
                if (_items.Count == 0)
                    return null;
                else
                    return _items[^1];
            }
        }

        public int Spacing
        {
            get => _Spacing;
            set
            {
                if (value < 0)
                    value = 0;
                if (_Spacing != value)
                {
                    _Spacing = value;
                    RequestUpdateFrame();
                }
            }
        }
        private int _Spacing;

        public abstract void AddedSubControlAndLayout(T control);

        public abstract void RemoveSubControlAndLayout(T control);
    }
}
