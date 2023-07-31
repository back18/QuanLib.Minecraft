using QuanLib.Minecraft.BlockScreen.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.FileExplorer
{
    public class FileExplorerApp : Application
    {
        public FileExplorerApp(string arguments) : base(arguments)
        {
            MainForm = new FileExplorerForm();
            ReturnValue = Array.Empty<string>();
            _exit = new(false);
        }

        private readonly AutoResetEvent _exit;

        public const string ID = "FileExplorer";

        public const string Name = "资源管理器";

        public override IForm MainForm { get; }

        internal string[] ReturnValue { get; set; }

        public override object? Main()
        {
            _exit.WaitOne();
            return ReturnValue;
        }

        public override void Exit()
        {
            _exit.Set();
            base.Exit();
        }
    }
}
