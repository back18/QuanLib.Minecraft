using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.TaskManager
{
    public class TaskManagerApp : Application
    {
        public TaskManagerApp(string arguments) : base(arguments)
        {
            MainForm = new TaskManagerForm();
            _exit = new(false);
        }

        private readonly AutoResetEvent _exit;

        public const string ID = "TaskManager";

        public const string Name = "任务管理器";

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
