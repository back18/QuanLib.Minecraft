using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.Settings
{
    public class SettingsApp : Application
    {
        public SettingsApp()
        {
            MainForm = new SettingsForm();
            _exit = new(false);
        }

        private readonly AutoResetEvent _exit;

        public const string ID = "Settings";

        public const string Name = "设置";

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
