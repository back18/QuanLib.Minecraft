using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public class ConcretePixel : ScreenPixel
    {
        static ConcretePixel()
        {
            _ids = new();
            for (int i = 0; i < 16; i++)
            {
                MinecraftColor color = (MinecraftColor)i;
                _ids.Add(color, $"minecraft:{CommandUtil.MinecraftColorToString(color)}_concrete");
            }
        }

        public ConcretePixel(Point position, MinecraftColor color) :
            base(position, _ids[color])
        {
            ConcreteColor = color;
        }

        private static readonly Dictionary<MinecraftColor, string> _ids;

        public MinecraftColor ConcreteColor { get; }

        public static string ToBlockID(MinecraftColor color)
            => _ids[color];
    }
}
