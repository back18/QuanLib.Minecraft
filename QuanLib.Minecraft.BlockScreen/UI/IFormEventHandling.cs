using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.UI
{
    public interface IFormEventHandling
    {
        public event EventHandler<IForm, EventArgs> FormLoad;

        public event EventHandler<IForm, EventArgs> FormClose;

        public event EventHandler<IForm, EventArgs> FormMinimize;

        public event EventHandler<IForm, EventArgs> FormUnminimize;

        public void HandleFormLoad(EventArgs e);

        public void HandleFormClose(EventArgs e);

        public void HandleFormMinimize(EventArgs e);

        public void HandleFormUnminimize(EventArgs e);
    }
}
