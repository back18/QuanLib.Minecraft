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
        public const string ID = "TaskManager";

        public const string Name = "任务管理器";

        public override object? Main(string[] args)
        {
            RunForm(new TaskManagerForm());
            return null;
        }
    }
}
