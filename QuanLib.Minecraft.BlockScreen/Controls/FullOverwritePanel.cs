using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Controls
{
    public class FullOverwritePanel<T> : Panel<T> where T : Control
    {
        private Frame? _frame;

        public override Frame RenderingFrame()
        {
            if (_frame is null || _frame.Width != Width || _frame.Height != Height)
                _frame = new(new string[Width, Height]);
            return _frame;
        }
    }
}
