using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Event
{
    public class ObjectChangedEventArgs<T> : EventArgs
    {
        public ObjectChangedEventArgs(T oldItem, T newItem)
        {
            OldItem = oldItem;
            NewItem = newItem;
        }

        public T OldItem { get; }

        public T NewItem { get; }
    }
}
