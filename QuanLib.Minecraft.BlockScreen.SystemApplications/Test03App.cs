using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications
{
    public class Test03App : Application
    {
        public Test03App(string arguments) : base(arguments)
        {
            MainForm = new Test03Form();
            _exit = new(false);
        }

        private readonly AutoResetEvent _exit;

        public const string ID = "Test03";

        public const string Name = "测试03";

        public override IForm MainForm { get; }

        public override object? Main()
        {
            _exit.WaitOne();
            return null;
        }

        public override void Exit()
        {
            _exit.Set();
            base.Exit();
        }
    }
}
