using QuanLib.Minecraft.BlockScreen.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BuiltInApps.DialogBox
{
    public class DialogBoxApp : Application
    {
        public DialogBoxApp()
        {
            MainForm = new MessageDialogBoxForm();
            ReturnValue = DialogBoxReturnValue.None;
            _exit = new(false);
        }

        private readonly AutoResetEvent _exit;

        public const string ID = "DialogBox";

        public const string Name = "对话框";

        public override string AppID => ID;

        public override string AppName => Name;

        public override Form MainForm { get; }

        internal DialogBoxReturnValue ReturnValue { get; set; }

        public override void Exit()
        {
            _exit.Set();
        }

        public override object? Main(string[] args)
        {
            _exit.WaitOne();
            return ReturnValue;
        }
    }
}
