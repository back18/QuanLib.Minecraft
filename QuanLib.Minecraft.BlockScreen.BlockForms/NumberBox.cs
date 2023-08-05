using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.BlockScreen.Frame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public class NumberBox : TextControl
    {
        public NumberBox()
        {
            FirstHandleCursorSlotChanged = true;
            ContentAnchor = AnchorPosition.Centered;

            _Value = 0;
            MinValue = int.MinValue;
            MaxValue = int.MaxValue;
            ScrollDelta = 1;
        }

        public int Value
        {
            get => _Value;
            set
            {
                if (value < MinValue)
                    value = MinValue;
                else if (value > MaxValue)
                    value = MaxValue;

                if (_Value != value)
                {
                    _Value = value;
                    Text = _Value.ToString();
                }
            }
        }
        private int _Value;

        public int MinValue { get; set; }

        public int MaxValue { get; set; }

        public int ScrollDelta { get; set; }

        protected override void OnCursorSlotChanged(Control sender, CursorSlotEventArgs e)
        {
            base.OnCursorSlotChanged(sender, e);

            Value += e.Delta * ScrollDelta;
        }
    }
}
