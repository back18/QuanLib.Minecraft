using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.Mspaint
{
    public class MspaintApp : Application
    {
        public MspaintApp()
        {
            MainForm = new MspaintForm();
            _exit = new(false);
        }

        private readonly AutoResetEvent _exit;

        public const string ID = "Mspaint";

        public const string Name = "画图";

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
