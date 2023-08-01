using NAudio.Codecs;
using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.BlockScreen.Frame;
using QuanLib.Minecraft.BlockScreen.UI;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Screens
{
    /// <summary>
    /// 屏幕运行时上下文
    /// </summary>
    public class ScreenContext
    {
        public ScreenContext(Screen screen, IRootForm form)
        {
            ID = -1;
            Screen = screen ?? throw new ArgumentNullException(nameof(screen));
            RootForm = form ?? throw new ArgumentNullException(nameof(form));
            CursorType = BlockScreen.CursorType.Default;
            _bind = false;
            BindEvents();
        }

        private bool _bind;

        public int ID { get; internal set; }

        public Screen Screen { get; }

        public IRootForm RootForm { get; set; }

        public string CursorType { get; set; }

        internal void BindEvents()
        {
            if (_bind)
                return;

            Screen.InputHandler.CursorMove += InputHandler_OnCursorMove;
            Screen.InputHandler.RightClick += InputHandler_OnRightClick;
            Screen.InputHandler.LeftClick += InputHandler_OnLeftClick;
            Screen.InputHandler.CursorItemChanged += InputHandler_CursorItemChanged;
            Screen.InputHandler.TextEditorChanged += InputHandler_OnTextEditorChanged;

            _bind = true;
        }

        internal void UnbindEvents()
        {
            if (!_bind)
                return;

            Screen.InputHandler.CursorMove -= InputHandler_OnCursorMove;
            Screen.InputHandler.RightClick -= InputHandler_OnRightClick;
            Screen.InputHandler.LeftClick -= InputHandler_OnLeftClick;
            Screen.InputHandler.TextEditorChanged -= InputHandler_OnTextEditorChanged;

            _bind = false;
        }

        private void InputHandler_OnCursorMove(ICursorReader sender, CursorEventArgs e)
        {
            RootForm.HandleCursorMove(e);
        }

        private void InputHandler_OnRightClick(ICursorReader sender, CursorEventArgs e)
        {
            RootForm.HandleRightClick(e);
        }

        private void InputHandler_OnLeftClick(ICursorReader sender, CursorEventArgs e)
        {
            RootForm.HandleLeftClick(e);
        }

        private void InputHandler_CursorItemChanged(ICursorReader sender, CursorItemEventArgs e)
        {
            RootForm.HandleCursorItemChanged(e);
        }

        private void InputHandler_OnTextEditorChanged(ICursorReader sender, CursorTextEventArgs e)
        {
            RootForm.HandleTextEditorChanged(e);
        }
    }
}
