using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.UI
{
    public interface IForm : IContainerControl, IFormEventHandling
    {
        public bool AllowSelected { get; }

        public bool AllowDeselected { get; }

        public bool AllowMove { get; set; }

        public bool AllowResize { get; set; }

        public bool Moveing { get; }

        public bool Resizeing { get; }

        public Direction ResizeBorder { get; }

        public object? ReturnValue { get; }
    }
}
