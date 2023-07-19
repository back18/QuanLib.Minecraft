using QuanLib.Minecraft.BlockScreen.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BuiltInApps.FileExplorer
{
    public class FileExplorerApp : Application
    {
        public FileExplorerApp()
        {
            MainForm = new FileExplorerForm();
            ReturnValue = Array.Empty<string>();
            _exit = new(false);
        }

        private readonly AutoResetEvent _exit;

        public const string ID = "FileExplorer";

        public const string Name = "资源管理器";

        public override string AppID => ID;

        public override string AppName => Name;

        public override Form MainForm { get; }

        internal string[] ReturnValue { get; set; }

        public override void Exit()
        {
            _exit.Set();
        }

        public override object? Main(string[] args)
        {
            _exit.WaitOne();
            return ReturnValue;
        }
    }
}
