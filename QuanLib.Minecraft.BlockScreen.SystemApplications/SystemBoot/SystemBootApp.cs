using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.SystemBoot
{
    public class SystemBootApp : Application
    {
        public SystemBootApp(string arguments) : base(arguments)
        {
            MainForm = new SystemBootForm();
        }

        public const string ID = "SystemBoot";

        public const string Name = "系统引导";

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
