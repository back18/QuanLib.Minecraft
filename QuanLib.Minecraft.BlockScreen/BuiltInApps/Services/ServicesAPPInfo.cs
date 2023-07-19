using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BuiltInApps.Services
{
    public class ServicesAppInfo : ApplicationInfo<ServicesApp>
    {
        public ServicesAppInfo()
        {
            Platforms = new PlatformID[]
            {
                PlatformID.Win32NT,
                PlatformID.Unix,
                PlatformID.MacOSX
            };
            ID = ServicesApp.ID;
            Name = ServicesApp.Name;
            Version = Version.Parse("1.0");
            Icon = DefaultIcon;
            AppendToDesktop = false;
        }

        public override PlatformID[] Platforms { get; }

        public override string ID { get; }

        public override string Name { get; }

        public override Version Version { get; }

        public override Image<Rgba32> Icon { get; }

        public override bool AppendToDesktop { get; }
    }
}
