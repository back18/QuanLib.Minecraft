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
            Skin.BackgroundBlockID_Selected = ConcretePixel.ToBlockID(MinecraftColor.LightBlue);
            Skin.BackgroundBlockID_Hover_Selected = ConcretePixel.ToBlockID(MinecraftColor.LightBlue);
            Skin.BorderBlockID_Selected = ConcretePixel.ToBlockID(MinecraftColor.Blue);
            Skin.BorderBlockID_Hover_Selected = ConcretePixel.ToBlockID(MinecraftColor.Blue);
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

        protected override void OnTextEditorChanged(Control sender, CursorTextEventArgs e)
        {
            base.OnTextEditorChanged(sender, e);

            Text = e.Text;
        }
    }
}
