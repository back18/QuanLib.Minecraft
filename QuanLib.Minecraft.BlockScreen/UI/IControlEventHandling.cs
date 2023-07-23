using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.UI
{
    public interface IControlEventHandling
    {
        public void HandleCursorMove(Point position, CursorMode mode);

        public bool HandleRightClick(Point position);

        public bool HandleLeftClick(Point position);

        public void HandleTextEditorUpdate(Point position, string text);

        public void HandleBeforeFrame();

        public void HandleAfterFrame();
    }
}
