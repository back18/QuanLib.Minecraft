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
using SixLabors.ImageSharp.PixelFormats;

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
            WallpaperPath = Path.Combine(MCOS.MainDirectory.Applications.GetApplicationDirectory(DesktopApp.ID), "wallpaper.jpg");

            ClientPanel = new();
        }

        public readonly ClientPanel ClientPanel;

        public string WallpaperPath { get; }

        public override void Initialize()
        {
            base.Initialize();

            SubControls.Add(ClientPanel);
            ClientPanel.ClientSize = ClientSize;
            ClientPanel.LayoutSyncer = new(this, (sender, e) => { }, (sender, e) =>
            ClientPanel.ClientSize = ClientSize);
            ClientPanel.LayoutAll += ClientPanel_LayoutAll;

            ActiveLayoutAll();

            ClientPanel.Skin.SetAllBackgroundImage(WallpaperPath);
        }

        public override void ActiveLayoutAll()
        {
            MCOS os = MCOS.Instance;
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

        public void SetAsWallpaper(Image<Rgba32> image)
        {
            if (image is null)
                throw new ArgumentNullException(nameof(image));

            ClientPanel.Skin.SetAllBackgroundImage(image);
            image.Save(WallpaperPath);
        }

        private void ClientPanel_LayoutAll(AbstractContainer<Control> sender, SizeChangedEventArgs e)
        {
            ActiveLayoutAll();
        }
    }
}
