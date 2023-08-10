using QuanLib.Minecraft.Block;
using QuanLib.Minecraft.BlockScreen.BlockForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.Notepad
{
    public class NotepadForm : WindowForm
    {
        public NotepadForm()
        {
            RichTextBox = new();
        }

        private readonly RichTextBox RichTextBox;

        public override void Initialize()
        {
            base.Initialize();

            ClientPanel.SubControls.Add(RichTextBox);
            RichTextBox.ClientLocation = new(2, 2);
            RichTextBox.Size = new(ClientPanel.ClientSize.Width - 4, ClientPanel.ClientSize.Height - 4);
            RichTextBox.Stretch = Direction.Bottom | Direction.Right;
            RichTextBox.WordWrap = false;
            RichTextBox.Text = File.ReadAllText(@"C:\Users\Administrator\Desktop\latest.log");
        }
    }
}
