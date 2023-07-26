using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.DialogBox
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

        public override IForm MainForm { get; }

        internal DialogBoxReturnValue ReturnValue { get; set; }

        public override object? Main(string[] args)
        {
            _exit.WaitOne();
            return ReturnValue;
        }

        public override void Exit()
        {
            _exit.Set();
            base.Exit();
        }
    }
}
