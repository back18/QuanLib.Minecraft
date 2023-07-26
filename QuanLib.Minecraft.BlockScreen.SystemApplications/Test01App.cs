using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications
{
    public class Test01App : Application
    {
        public Test01App()
        {
            MainForm = new Test01Form();
            _exit = new(false);
        }

        private readonly AutoResetEvent _exit;

        public const string ID = "Test01";

        public const string Name = "测试01";

        public override IForm MainForm { get; }

        public override object? Main(string[] args)
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
