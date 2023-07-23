using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.UI.Controls
{
    public class FullOverwritePanel<T> : Panel<T> where T : Control
    {
        private ArrayFrame? _frame;

        public override AbstractFrame RenderingFrame()
        {
            if (_frame is null || _frame.Width != Width || _frame.Height != Height)
                _frame = new(new string[Width, Height]);
            return _frame;
        }
    }
}
