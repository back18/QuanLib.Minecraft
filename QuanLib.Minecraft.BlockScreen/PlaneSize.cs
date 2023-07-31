using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public readonly struct PlaneSize : IPlaneSize
    {
        public PlaneSize(int width, int height, Facing normalFacing)
        {
            Width = width;
            Height = height;
            NormalFacing = normalFacing;
        }

        public int Width { get; }

        public int Height { get; }

        public Facing NormalFacing { get; }

        public static bool TryParse(string s, out PlaneSize result)
        {
            if (string.IsNullOrEmpty(s))
                goto err;

            string[] items = s.Split(' ');
            if (items.Length != 3)
                goto err;

            if (int.TryParse(items[0], out var width) &&
                int.TryParse(items[1], out var height) &&
                Enum.TryParse(typeof(Facing), items[2], out var @enum) &&
                @enum is Facing facing)
            {
                result = new(width, height, facing);
                return true;
            }

            err:
            result = default;
            return false;
        }
    }
}
