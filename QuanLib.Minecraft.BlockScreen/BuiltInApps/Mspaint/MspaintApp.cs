using QuanLib.Minecraft.BlockScreen.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BuiltInApps.Mspaint
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

        public override string AppID => ID;

        public override string AppName => Name;

        public override Form MainForm { get; }

        public override object? Main(string[] args)
        {
            _exit.WaitOne();
            return null;
        }

        public override void Exit()
        {
            _exit.Set();
        }
    }
}
