using QuanLib.Minecraft.BlockScreen.Event;
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
        public void HandleCursorMove(CursorEventArgs e);

        public bool HandleRightClick(CursorEventArgs e);

        public bool HandleLeftClick(CursorEventArgs e);

        public void HandleCursorItemChanged(CursorItemEventArgs e);

        public void HandleTextEditorChanged(CursorTextEventArgs e);

        public void HandleBeforeFrame(EventArgs e);

        public void HandleAfterFrame(EventArgs e);
    }
}
