using QuanLib.Minecraft.BlockScreen.BlockForms;
using QuanLib.Minecraft.BlockScreen.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications
{
    public class Test03Form : WindowForm
    {
        public override void Initialize()
        {
            base.Initialize();

            ClientPanel.Skin.SetAllBackgroundBlockID(ConcretePixel.ToBlockID(MinecraftColor.Lime));
        }

        protected override void OnCursorSlotChanged(Control sender, CursorSlotEventArgs e)
        {
            base.OnCursorSlotChanged(sender, e);

            Console.WriteLine(e.Delta);
        }
    }
}
