using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.UI
{
    public interface IForm : IContainerControl, IApplicationComponent
    {
        public bool AllowSelected { get; }

        public bool AllowDeselected { get; }

        public bool AllowMove { get; set; }

        public bool AllowResize { get; }

        public bool Moveing { get; }

        public bool Resizeing { get; }

        public Direction ResizeBorder { get; }

        public bool IsMaximize { get; }

        public event Action<IForm> OnFormClose;

        public void MaximizeForm();

        public void RestoreForm();

        public void CloseForm();
    }
}
