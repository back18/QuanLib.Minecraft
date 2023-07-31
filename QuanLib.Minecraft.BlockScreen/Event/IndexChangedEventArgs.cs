using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Event
{
    public class IndexChangedEventArgs : EventArgs
    {
        public IndexChangedEventArgs(int oldIndex, int newIndex)
        {
            OldIndex = oldIndex;
            NewIndex = newIndex;
        }

        public int OldIndex { get; }

        public int NewIndex { get; }
    }
}
