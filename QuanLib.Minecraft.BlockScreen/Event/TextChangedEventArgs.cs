using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Event
{
    public class TextChangedEventArgs : EventArgs
    {
        public TextChangedEventArgs(string oldText, string newText)
        {
            OldText = oldText;
            NewText = newText;
        }

        public string OldText { get; }

        public string NewText { get; }
    }
}
