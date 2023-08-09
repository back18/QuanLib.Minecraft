using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public class ListMenuBox<T> : MenuBox<T> where T : Control
    {
        public override void AddedSubControlAndLayout(T control)
        {
            SubControls.Add(control);
            if (PreviousSubControl is not null)
                control.ClientLocation = this.BottomLayout(PreviousSubControl, Spacing);
            else
                control.ClientLocation = new(Spacing, Spacing);
            _items.Add(control);

            PageSize = new(ClientSize.Width, control.BottomLocation + 1 + Spacing);
        }

        public override void RemoveSubControlAndLayout(T control)
        {
            throw new NotImplementedException();
        }
    }
}
