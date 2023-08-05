using QuanLib.Minecraft.BlockScreen.UI;
using QuanLib.Minecraft.BlockScreen.BlockForms;
using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using QuanLib.Minecraft.BlockScreen.Event;
using System.Diagnostics;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.Desktop
{
    public class DesktopForm : Form
    {
        public DesktopForm()
        {
            AllowMove = false;
            AllowResize = false;
            DisplayPriority = int.MinValue;
            MaxDisplayPriority = int.MinValue + 1;
            BorderWidth = 0;

            ClientPanel = new();
        }

        public readonly ClientPanel ClientPanel;

        public override void Initialize()
        {
            base.Initialize();

            SubControls.Add(ClientPanel);
            ClientPanel.ClientSize = ClientSize;
            ClientPanel.LayoutSyncer = new(this, (sender, e) => { }, (sender, e) =>
            ClientPanel.ClientSize = ClientSize);
            ClientPanel.LayoutAll += ClientPanel_LayoutAll;

            ActiveLayoutAll();

            ClientPanel.Skin.SetAllBackgroundImage(new("C:\\Users\\Administrator\\Desktop\\1.jpg", GetScreenPlaneSize().NormalFacing, ClientPanel.GetRenderingSize()));
        }

        private void ClientPanel_LayoutAll(AbstractContainer<Control> sender, SizeChangedEventArgs e)
        {
            ActiveLayoutAll();
        }

        public override void ActiveLayoutAll()
        {
            MCOS os = MCOS.GetMCOS();
            ClientPanel.SubControls.Clear();
            foreach (var app in os.ApplicationManager.ApplicationList.Values)
                if (app.AppendToDesktop)
                    ClientPanel.SubControls.Add(new DesktopIcon(app));

            if (ClientPanel.SubControls.Count == 0)
                return;

            ClientPanel.ForceFillRightLayout(0, ClientPanel.SubControls);
            ClientPanel.PageSize = new((ClientPanel.SubControls.RecentlyAddedControl ?? ClientPanel.SubControls[^1]).RightLocation + 1, ClientPanel.ClientSize.Height);
            ClientPanel.OffsetPosition = new(0, 0);
            ClientPanel.RefreshHorizontalScrollBar();
        }
    }
}
