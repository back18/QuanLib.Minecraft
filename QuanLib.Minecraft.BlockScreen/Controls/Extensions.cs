using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Controls
{
    public static class Extensions
    {
        public static int IndexOf<T>(this IReadOnlyControlCollection<T> source, T item) where T : Control
        {
            for (int i = 0; i < source.Count; i++)
            {
                if (source[i] == item)
                    return i;
            }
            return -1;
        }
    }
}
