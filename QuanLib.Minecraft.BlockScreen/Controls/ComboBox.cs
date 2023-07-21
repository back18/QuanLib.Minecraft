using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Controls
{
    public class ComboBox<T> : Control, IItemContainer<T> where T : notnull
    {
        public ComboBox()
        {
            Items = new();
        }

        public ItemCollection<T> Items { get; }
    }
}
