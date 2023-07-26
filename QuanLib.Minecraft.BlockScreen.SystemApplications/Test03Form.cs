using QuanLib.Minecraft.BlockScreen.BlockForms;
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

            Client_Panel.Skin.SetAllBackgroundBlockID(ConcretePixel.ToBlockID(MinecraftColor.Lime));
        }
    }
}
