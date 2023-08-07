using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using QuanLib.Minecraft.BlockScreen.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public class MenuBox : ScrollablePanel
    {
        public void AddedSubControlAndLayout(Control control)
        {
            Control? previous = SubControls.RecentlyAddedControl;
            SubControls.Add(control);
            if (previous is not null)
                control.ClientLocation = this.BottomLayout(previous, 1);
            else
                control.ClientLocation = new(1, 1);

            PageSize = new(ClientSize.Width, control.BottomLocation + 2);
        }
    }
}
