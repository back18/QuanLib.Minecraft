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
        public const string ID = "FileExplorer";

        public const string Name = "资源管理器";

        public override object? Main(string[] args)
        {
            RunForm(new FileExplorerForm());
            return null;
        }
    }
}
