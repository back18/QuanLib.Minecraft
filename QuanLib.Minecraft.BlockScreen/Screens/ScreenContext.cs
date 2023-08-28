using static QuanLib.Minecraft.BlockScreen.Config.ConfigManager;
using log4net.Core;
using NAudio.Codecs;
using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.BlockScreen.Frame;
using QuanLib.Minecraft.BlockScreen.Logging;
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
        private static readonly LogImpl LOGGER = LogUtil.MainLogger;

        internal ScreenContext(Screen screen, IRootForm form)
        {
            ScreenState = ScreenState.NotLoaded;
            ID = -1;
            Screen = screen ?? throw new ArgumentNullException(nameof(screen));
            RootForm = form ?? throw new ArgumentNullException(nameof(form));
            IsRestart = false;
            IsShowCursor = true;
            CursorType = BlockScreen.CursorType.Default;
            _bind = false;
        }

        private bool _bind;

        public ScreenState ScreenState { get; private set; }

        public bool IsRestart { get; private set; }

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
            Screen.InputHandler.TextEditorUpdate += InputHandler_TextEditorUpdate;

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
            Screen.InputHandler.TextEditorUpdate -= InputHandler_TextEditorUpdate;

            _bind = false;
        }

        public void Handle()
        {
            switch (ScreenState)
            {
                case ScreenState.NotLoaded:
                    break;
                case ScreenState.Loading:
                    Screen.Start();
                    RootForm.ClientSize = Screen.Size;
                    MCOS.Instance.RunStartupChecklist(RootForm);
                    BindEvents();
                    ScreenState = ScreenState.Active;
                    LOGGER.Info($"屏幕“{ToString()}”已加载");
                    break;
                case ScreenState.Active:
                    if (ScreenConfig.ScreenIdleTimeout != -1 && Screen.InputHandler.IdleTime >= ScreenConfig.ScreenIdleTimeout)
                    {
                        ScreenState = ScreenState.Closed;
                        LOGGER.Warn($"ID为{ID}的屏幕已达到最大闲置时间，即将卸载");
                        goto case ScreenState.Closed;
                    }
                    break;
                case ScreenState.Sleep:
                    //TODO
                    break;
                case ScreenState.Closed:
                    UnbindEvents();
                    foreach (var forem in MCOS.Instance.FormManager.Items.Values)
                    {
                        if (forem.RootForm == RootForm)
                            forem.CloseForm();
                    }
                    RootForm.CloseForm();
                    Screen.Stop();
                    LOGGER.Info($"屏幕“{ToString()}”已卸载");
                    break;
                default:
                    break;
            }
        }

        public ScreenContext LoadScreen()
        {
            if (ScreenState == ScreenState.NotLoaded)
            {
                ScreenState = ScreenState.Loading;
            }
            return this;
        }

        public void CloseScreen()
        {
            ScreenState = ScreenState.Closed;
        }

        public void RestartScreen()
        {
            ScreenState = ScreenState.Closed;
            IsRestart = true;
        }

        public void StartSleep()
        {
            ScreenState = ScreenState.Sleep;
        }

        public void StopSleep()
        {
            ScreenState = ScreenState.Active;
        }

        public override string ToString()
        {
            return $"State={ScreenState}, SID={ID}, Screen=[{Screen}]";
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

        private void InputHandler_TextEditorUpdate(ITextEditor sender, CursorTextEventArgs e)
        {
            RootForm.HandleTextEditorUpdate(e);
        }
    }
}
