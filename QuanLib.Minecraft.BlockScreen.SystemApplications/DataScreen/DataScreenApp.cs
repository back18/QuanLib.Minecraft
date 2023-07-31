using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.DataScreen
{
    public class DataScreenApp : Application
    {
        public DataScreenApp(string arguments) : base(arguments)
        {
            MainForm = new DataScreenForm();
            _exit = new(false);
        }

        private readonly AutoResetEvent _exit;

        public const string ID = "DataScreen";

        public const string Name = "数据大屏";

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
