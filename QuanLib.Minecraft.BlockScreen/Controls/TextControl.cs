using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Controls
{
    public abstract class TextControl : Control
    {
        protected TextControl()
        {
            OnTextUpdate += TextControl_OnTextUpdate;
        }

        public override void AutoSetSize()
        {
            ClientSize = MCOS.DefaultFont.GetTotalSize(Text);
        }

        private void TextControl_OnTextUpdate(string oldText, string newText)
        {
            if (AutoSize)
                AutoSetSize();
        }
    }
}
