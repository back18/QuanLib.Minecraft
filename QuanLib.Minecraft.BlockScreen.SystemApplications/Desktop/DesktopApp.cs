using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.Desktop
{
    public class DesktopApp : Application
    {
        public const string ID = "Desktop";

        public const string Name = "系统桌面";

        public override object? Main(string[] args)
        {
            RunForm(new DesktopForm());
            return null;
        }
    }
}
