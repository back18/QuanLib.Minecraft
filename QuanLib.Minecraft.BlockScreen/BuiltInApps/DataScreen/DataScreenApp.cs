using QuanLib.Minecraft.BlockScreen.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BuiltInApps.DataScreen
{
    public class DataScreenApp : Application
    {
        public DataScreenApp()
        {
            MainForm = new DataScreenForm();
            _exit = new(false);
        }

        private readonly AutoResetEvent _exit;

        public const string ID = "DataScreen";

        public const string Name = "数据大屏";

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
