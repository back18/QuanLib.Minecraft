using QuanLib.Minecraft.Block;
using QuanLib.Minecraft.BlockScreen.Event;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public class TextBox : TextControl
    {
        public TextBox()
        {
            ClientSize = new(64, 16);
            Skin.BackgroundBlockID_Selected = BlockManager.Concrete.LightBlue;
            Skin.BackgroundBlockID_Hover_Selected = BlockManager.Concrete.LightBlue;
            Skin.BorderBlockID_Selected = BlockManager.Concrete.Blue;
            Skin.BorderBlockID_Hover_Selected = BlockManager.Concrete.Blue;
        }

        protected override void OnCursorEnter(Control sender, CursorEventArgs e)
        {
            base.OnCursorEnter(sender, e);

            if (GetScreenContext()?.Screen.InputHandler.CurrenMode == CursorMode.TextEditor)
            {
                IsSelected = true;
                SetTextEditorInitialText();
                ResetTextEditor();
            }
        }

        protected override void OnCursorLeave(Control sender, CursorEventArgs e)
        {
            base.OnCursorLeave(sender, e);

            if (IsSelected)
                IsSelected = false;
        }

        protected override void OnTextEditorUpdate(Control sender, CursorTextEventArgs e)
        {
            base.OnTextEditorUpdate(sender, e);

            Text = e.Text;
        }
    }
}
