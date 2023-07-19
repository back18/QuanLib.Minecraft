using QuanLib.Minecraft.BlockScreen;
using QuanLib.Minecraft.BlockScreen.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BuiltInApps.Desktop
{
    public class DesktopApp : Application
    {
        public DesktopApp()
        {
            MainForm = new DesktopForm();
        }

        public const string ID = "Desktop";

        public const string Name = "系统桌面";

        public override string AppID => ID;

        public override string AppName => Name;

        public override Form MainForm { get; }

        public override object? Main(string[] args)
        {
            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        public override void Exit()
        {
            Console.WriteLine("尝试关机");
        }
    }
}
