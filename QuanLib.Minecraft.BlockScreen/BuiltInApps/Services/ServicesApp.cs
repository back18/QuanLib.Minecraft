using QuanLib.Minecraft.BlockScreen.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BuiltInApps.Services
{
    public class ServicesApp : Application
    {
        public ServicesApp()
        {
            RootForm = new RootForm();
            MainForm = RootForm;
        }

        public const string ID = "Services";

        public const string Name = "系统服务";

        public override string AppID => ID;

        public override string AppName => Name;

        public override Form MainForm { get; }

        public RootForm RootForm { get; }

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
            //base.Exit();
        }
    }
}
