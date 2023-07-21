using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Controls
{
    public interface IItemContainer<T> where T : notnull
    {
        public ItemCollection<T> Items { get; }
    }
}
