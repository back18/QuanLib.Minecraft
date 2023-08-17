using QuanLib.Minecraft.Block;
using QuanLib.Minecraft.BlockScreen.Event;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public class TextBox : TextControl
    {
        public TextBox()
        {
            IsReadOnly = false;

            ClientSize = new(64, 16);
            Skin.BackgroundBlockID_Selected = BlockManager.Concrete.LightBlue;
            Skin.BackgroundBlockID_Hover_Selected = BlockManager.Concrete.LightBlue;
        }

        public bool IsReadOnly { get; set; }

        protected override void OnCursorMove(Control sender, CursorEventArgs e)
        {
            base.OnCursorMove(sender, e);

            HandleInput();
        }

        protected override void OnCursorEnter(Control sender, CursorEventArgs e)
        {
            base.OnCursorEnter(sender, e);

            HandleInput();
        }

        protected override void OnCursorLeave(Control sender, CursorEventArgs e)
        {
            base.OnCursorLeave(sender, e);

            IsSelected = false;
        }

        protected override void OnTextEditorUpdate(Control sender, CursorTextEventArgs e)
        {
            base.OnTextEditorUpdate(sender, e);

            if (!IsReadOnly)
                Text = e.Text;
        }

        private void HandleInput()
        {
            if (!IsReadOnly && GetScreenContext()?.Screen.InputHandler.CurrenMode == CursorMode.TextEditor)
            {
                IsSelected = true;
                SetTextEditorInitialText();
                ResetTextEditor();
            }
            else
            {
                IsSelected = false;
            }
        }
    }
}
