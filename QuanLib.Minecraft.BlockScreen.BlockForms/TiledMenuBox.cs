using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public class TiledMenuBox<T> : MenuBox<T> where T : Control
    {
        public TiledMenuBox()
        {
            _MinWidth = 1;
        }

        public int MinWidth
        {
            get => _MinWidth;
            set
            {
                if (value < 1)
                    value = 1;

                if (_MinWidth != value)
                {
                    _MinWidth = value;
                    RequestUpdateFrame();
                }
            }
        }
        private int _MinWidth;

        public override void AddedSubControlAndLayout(T control)
        {
            SubControls.Add(control);
            if (PreviousSubControl is not null)
                control.ClientLocation = this.RightLayout(PreviousSubControl, Spacing);
            else
                control.ClientLocation = new(Spacing, Spacing);
            _items.Add(control);

            if (control.RightToBorder < 0)
            {
                ActiveLayoutAll();
            }
        }

        public override void RemoveSubControlAndLayout(T control)
        {
            SubControls.Remove(control);

            if (control != PreviousSubControl)
            {
                _items.Remove(control);
                ActiveLayoutAll();
            }
            else
            {
                _items.Remove(control);
            }
        }

        public override void ActiveLayoutAll()
        {
            int totalWidth = 0;
            foreach (var control in _items)
                totalWidth += control.Width;

            if (totalWidth > ClientSize.Width)
            {
                int width = ClientSize.Width / _items.Count;
                if (width < MinWidth)
                {
                    width = MinWidth;
                    PageSize = new(Math.Max(MinWidth * _items.Count, ClientSize.Width), ClientSize.Height);
                }
                foreach (var item in _items)
                    item.Width = width;
            }
            else
            {
                PageSize = ClientSize;
            }

            T? previous = null;
            foreach (var item in _items)
            {
                if (previous is not null)
                    item.ClientLocation = this.RightLayout(previous, Spacing);
                else
                    item.ClientLocation = new(Spacing, Spacing);
                previous = item;
            }
        }
    }
}
