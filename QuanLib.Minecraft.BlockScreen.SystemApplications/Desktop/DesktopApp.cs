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
        public DesktopApp(string arguments) : base(arguments)
        {
            MainForm = new DesktopForm();
        }

        public const string ID = "Desktop";

        public const string Name = "系统桌面";

        public override IForm MainForm { get; }

        public override object? Main()
        {
            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        public override void Exit()
        {
            Console.WriteLine("尝试关机");
            //base.Exit();
        }
    }
}
