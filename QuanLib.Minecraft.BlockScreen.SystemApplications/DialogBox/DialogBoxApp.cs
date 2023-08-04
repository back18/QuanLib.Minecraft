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
        public const string ID = "DialogBox";

        public const string Name = "对话框";

        public override object? Main(string[] args)
        {
            RunForm(new MessageDialogBoxForm());
            return null;
        }
    }
}
