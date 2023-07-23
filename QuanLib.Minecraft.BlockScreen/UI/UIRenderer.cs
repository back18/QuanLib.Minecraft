using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.UI
{
    public abstract class UIRenderer<T> where T : IUIRendering
    {
        public abstract ArrayFrame? Rendering(T rendering);
    }
}
