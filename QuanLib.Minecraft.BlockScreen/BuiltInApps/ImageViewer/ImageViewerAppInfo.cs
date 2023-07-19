using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BuiltInApps.ImageViewer
{
    public class ImageViewerAppInfo : ApplicationInfo<ImageViewerApp>
    {
        public ImageViewerAppInfo()
        {
            Platforms = new PlatformID[]
            {
                PlatformID.Win32NT,
                PlatformID.Unix,
                PlatformID.MacOSX
            };
            ID = ImageViewerApp.ID;
            Name = ImageViewerApp.Name;
            Version = Version.Parse("1.0");
            try
            {
                Icon = Image.Load<Rgba32>(Path.Combine(PathManager.GetApplicationDir(ID), "Icon.png"));
            }
            catch
            {
                Icon = DefaultIcon;
            }
            AppendToDesktop = true;
        }

        public override PlatformID[] Platforms { get; }

        public override string ID { get; }

        public override string Name { get; }

        public override Version Version { get; }

        public override Image<Rgba32> Icon { get; }

        public override bool AppendToDesktop { get; }
    }
}
