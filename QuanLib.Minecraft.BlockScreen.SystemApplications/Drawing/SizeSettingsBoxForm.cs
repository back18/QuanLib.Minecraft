using QuanLib.Minecraft.BlockScreen.BlockForms;
using QuanLib.Minecraft.BlockScreen.BlockForms.DialogBox;
using QuanLib.Minecraft.BlockScreen.BlockForms.Utility;
using QuanLib.Minecraft.BlockScreen.UI;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.Drawing
{
    public class SizeSettingsBoxForm : DialogBoxForm<Size>
    {
        public SizeSettingsBoxForm(IForm initiator, string title, Size initial) : base(initiator, title)
        {
            Width_Label = new();
            Width_TextBox = new();
            Height_Label = new();
            Height_TextBox = new();
            OK_Button = new();
            Cancel_Button = new();

            _initial = initial;
            DefaultResult = new(0, 0);
            DialogResult = DefaultResult;
        }

        private readonly Size _initial;

        private readonly Label Width_Label;

        private readonly TextBox Width_TextBox;

        private readonly Label Height_Label;

        private readonly TextBox Height_TextBox;

        private readonly Button OK_Button;

        private readonly Button Cancel_Button;

        public override Size DefaultResult { get; }

        public override Size DialogResult { get; protected set; }

        public override void Initialize()
        {
            base.Initialize();

            ClientSize = new(104, 62 + TitleBar.Height);
            CenterOnInitiatorForm();

            ClientPanel.SubControls.Add(Width_Label);
            Width_Label.Text = "宽度";
            Width_Label.ClientLocation = new(2, 3);

            ClientPanel.SubControls.Add(Height_Label);
            Height_Label.Text = "高度";
            Height_Label.ClientLocation = ClientPanel.BottomLayout(Width_Label, 4);

            ClientPanel.SubControls.Add(Width_TextBox);
            Width_TextBox.Text = _initial.Width.ToString();
            Width_TextBox.ClientLocation = new(Width_Label.RightLocation + 3, 2);
            Width_TextBox.ClientSize = new(64, 16);

            ClientPanel.SubControls.Add(Height_TextBox);
            Height_TextBox.Text = _initial.Height.ToString();
            Height_TextBox.ClientLocation = ClientPanel.BottomLayout(Width_TextBox, 2);
            Height_TextBox.ClientSize = new(64, 16);

            ClientPanel.SubControls.Add(Cancel_Button);
            Cancel_Button.Text = "取消";
            Cancel_Button.ClientSize = new(32, 16);
            Cancel_Button.ClientLocation = ClientPanel.LeftLayout(null, Cancel_Button, 2, Height_TextBox.BottomLocation + 3);
            Cancel_Button.RightClick += Cancel_Button_RightClick;

            ClientPanel.SubControls.Add(OK_Button);
            OK_Button.Text = "确认";
            OK_Button.ClientSize = new(32, 16);
            OK_Button.ClientLocation = ClientPanel.LeftLayout(Cancel_Button, OK_Button, 2);
            OK_Button.RightClick += OK_Button_RightClick;
        }

        private void OK_Button_RightClick(Control sender, Event.CursorEventArgs e)
        {
            if (!int.TryParse(Width_TextBox.Text, out var width) || !int.TryParse(Height_TextBox.Text, out var heigth))
            {
                return;
            }
            DialogResult = new(width, heigth);
            CloseForm();
        }

        private void Cancel_Button_RightClick(Control sender, Event.CursorEventArgs e)
        {
            CloseForm();
        }
    }
}
