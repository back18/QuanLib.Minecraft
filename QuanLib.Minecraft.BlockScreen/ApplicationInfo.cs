using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public abstract class ApplicationInfo
    {
        static ApplicationInfo()
        {
            DefaultIcon = Image.Load<Rgba32>(Path.Combine(MCOS.MainDirectory.SystemResources.Textures.Icon, "DefaultIcon.png"));
        }

        protected ApplicationInfo(Type typeObject)
        {
            TypeObject = typeObject;
        }

        public static Image<Rgba32> DefaultIcon { get; }

        public Type TypeObject { get; }

        public abstract PlatformID[] Platforms { get; }

        public abstract string ID { get; }

        public abstract string Name { get; }

        public abstract Version Version { get; }

        public abstract Image<Rgba32> Icon { get; }

        public abstract bool AppendToDesktop { get; }

        public override string ToString()
        {
            return $"Name={Name}, ID={ID}, Version={Version}";
        }
    }

    public abstract class ApplicationInfo<T> : ApplicationInfo where T : Application
    {
        protected ApplicationInfo() : base(typeof(T)) { }
    }
}
