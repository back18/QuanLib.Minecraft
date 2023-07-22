using QuanLib.Minecraft.BlockScreen.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BuiltInApps.Desktop
{
    public class DesktopForm : Form
    {
        public DesktopForm()
        {
            AllowResize = false;
            DisplayPriority = int.MinValue;
            MaxDisplayPriority = int.MinValue + 1;
            BorderWidth = 0;
            Skin.SetAllBackgroundBlockID(ConcretePixel.ToBlockID(MinecraftColor.Pink));
            //Skin.SetAllBackgroundBlockID("minecraft:air");
        }

        public override void Initialize()
        {
            base.Initialize();

            UpdateAppList();
        }

        private void UpdateAppList()
        {
            MCOS os = GetMCOS();
            SubControls.Clear();
            foreach (var app in os.ApplicationList.Values)
                if (app.AppendToDesktop)
                    SubControls.Add(new DesktopIcon(app));
            this.FillLayout(0, SubControls, 0);
        }
    }
}
