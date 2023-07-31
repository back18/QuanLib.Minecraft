using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.Services
{
    public class ServicesApp : ServicesApplication
    {
        public ServicesApp(string arguments) : base(arguments)
        {
            RootForm = new ServicesForm();
            MainForm = RootForm;
        }

        public const string ID = "Services";

        public const string Name = "系统服务";

        public override IForm MainForm { get; }

        public override IRootForm RootForm { get; }

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
