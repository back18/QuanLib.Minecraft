using QuanLib.Minecraft.BlockScreen.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BuiltInApps.TaskManager
{
    public class TaskManagerApp : Application
    {
        public TaskManagerApp()
        {
            MainForm = new TaskManagerForm();
            _exit = new(false);
        }

        private readonly AutoResetEvent _exit;

        public const string ID = "TaskManager";

        public const string Name = "任务管理器";

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
            base.Exit();
        }
    }
}
