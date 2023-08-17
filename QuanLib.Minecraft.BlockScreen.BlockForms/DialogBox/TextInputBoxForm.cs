using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms.DialogBox
{
    public class TextInputBoxForm : DialogBoxForm<string>
    {
        public TextInputBoxForm(IForm initiator, string title) : base(initiator, title)
        {
            DefaultResult = string.Empty;
            DialogResult = DefaultResult;

            Text_RichTextBox = new();
            OK_Button = new();
            Cancel_Button = new();
        }

        private readonly RichTextBox Text_RichTextBox;

        private readonly Button OK_Button;

        private readonly Button Cancel_Button;

        public override string DefaultResult { get; }

        public override string DialogResult { get; protected set; }

        public override void Initialize()
        {
            base.Initialize();

            ClientSize = new(102, 74 + TitleBar.Height);
            CenterOnInitiatorForm();

            ClientPanel.SubControls.Add(Text_RichTextBox);
            Text_RichTextBox.IsReadOnly = false;
            Text_RichTextBox.ClientLocation = new(2, 2);
            Text_RichTextBox.ClientSize = new(96, 48);

            ClientPanel.SubControls.Add(Cancel_Button);
            Cancel_Button.Text = "取消";
            Cancel_Button.ClientSize = new(32, 16);
            Cancel_Button.ClientLocation = ClientPanel.LeftLayout(null, Cancel_Button, 2, Text_RichTextBox.BottomLocation + 3);
            Cancel_Button.RightClick += Cancel_Button_RightClick;

            ClientPanel.SubControls.Add(OK_Button);
            OK_Button.Text = "确认";
            OK_Button.ClientSize = new(32, 16);
            OK_Button.ClientLocation = ClientPanel.LeftLayout(Cancel_Button, OK_Button, 2);
            OK_Button.RightClick += OK_Button_RightClick;
        }

        private void OK_Button_RightClick(Control sender, Event.CursorEventArgs e)
        {
            DialogResult = Text_RichTextBox.Text;
            CloseForm();
        }

        private void Cancel_Button_RightClick(Control sender, Event.CursorEventArgs e)
        {
            CloseForm();
        }
    }
}
