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
            ScreenState = ScreenState.NotLoaded;
            ID = -1;
            Screen = screen ?? throw new ArgumentNullException(nameof(screen));
            RootForm = form ?? throw new ArgumentNullException(nameof(form));
            IsShowCursor = true;
            CursorType = BlockScreen.CursorType.Default;
            _bind = false;
            BindEvents();
        }

        private bool _bind;

        public ScreenState ScreenState { get; private set; }

        public int ID { get; internal set; }

        public Screen Screen { get; }

        public IRootForm RootForm { get; set; }

        public bool IsShowCursor { get; set; }

        public string CursorType { get; set; }

        internal void BindEvents()
        {
            if (_bind)
                return;

            Screen.InputHandler.CursorMove += InputHandler_CursorMove;
            Screen.InputHandler.RightClick += InputHandler_RightClick;
            Screen.InputHandler.LeftClick += InputHandler_LeftClick;
            Screen.InputHandler.CursorSlotChanged += InputHandler_CursorSlotChanged;
            Screen.InputHandler.CursorItemChanged += InputHandler_CursorItemChanged;
            Screen.InputHandler.TextEditorChanged += InputHandler_TextEditorChanged;

            _bind = true;
        }

        internal void UnbindEvents()
        {
            if (!_bind)
                return;

            Screen.InputHandler.CursorMove -= InputHandler_CursorMove;
            Screen.InputHandler.RightClick -= InputHandler_RightClick;
            Screen.InputHandler.LeftClick -= InputHandler_LeftClick;
            Screen.InputHandler.CursorSlotChanged -= InputHandler_CursorSlotChanged;
            Screen.InputHandler.CursorItemChanged -= InputHandler_CursorItemChanged;
            Screen.InputHandler.TextEditorChanged -= InputHandler_TextEditorChanged;

            _bind = false;
        }

        public void LoadScreen()
        {
            Screen.Start();
            ScreenState = ScreenState.Active;
        }

        public void CloseScreen()
        {
            foreach (var forem in RootForm.GetAllForm().ToArray())
                MCOS.Instance.FormContextOf(forem)?.CloseForm();
            MCOS.Instance.FormContextOf(RootForm)?.CloseForm();
            Screen.Stop();

            ScreenState = ScreenState.Closed;
        }

        public void StartSleep()
        {
            ScreenState = ScreenState.Sleep;
        }

        public void StopSleep()
        {
            ScreenState = ScreenState.Active;
        }

        private void InputHandler_CursorMove(ICursorReader sender, CursorEventArgs e)
        {
            RootForm.HandleCursorMove(e);
        }

        private void InputHandler_RightClick(ICursorReader sender, CursorEventArgs e)
        {
            RootForm.HandleRightClick(e);
        }

        private void InputHandler_CursorSlotChanged(ICursorReader sender, CursorSlotEventArgs e)
        {
            RootForm.HandleCursorSlotChanged(e);
        }

        private void InputHandler_LeftClick(ICursorReader sender, CursorEventArgs e)
        {
            RootForm.HandleLeftClick(e);
        }

        private void InputHandler_CursorItemChanged(ICursorReader sender, CursorItemEventArgs e)
        {
            RootForm.HandleCursorItemChanged(e);
        }

        private void InputHandler_TextEditorChanged(ITextEditor sender, CursorTextEventArgs e)
        {
            RootForm.HandleTextEditorChanged(e);
        }
    }
}
