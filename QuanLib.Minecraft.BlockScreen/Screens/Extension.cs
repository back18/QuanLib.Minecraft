using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Screens
{
    public static class Extension
    {
        public static bool EqualsScreenOption(this IScreenOptions source, IScreenOptions? other)
        {
            if (source is null && other is null)
                return true;

            if (source is null || other is null)
                return false;

            return source.StartPosition.Equals(other.StartPosition)
                    && source.Width == other.Width
                    && source.Height == other.Height
                    && source.XFacing == other.XFacing
                    && source.YFacing == other.YFacing;
        }

        public static bool ContainsScreenOption<T>(this IEnumerable<T> source, IScreenOptions value) where T : IScreenOptions
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            return source.Any(item => value.EqualsScreenOption(item));
        }
    }
}
