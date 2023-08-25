using QuanLib.Minecraft.BlockScreen.SystemApplications.FileExplorer.Config;
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

        public FileExplorerConfig FileExplorerConfig
        {
            get
            {
                if (_FileExplorerConfig is null)
                    throw new InvalidOperationException();
                return _FileExplorerConfig;
            }
        }
        private FileExplorerConfig? _FileExplorerConfig;

        public override object? Main(string[] args)
        {
            FileExplorerConfig.CreateIfNotExists();
            _FileExplorerConfig = FileExplorerConfig.Load();

            string? path = null;
            if (args.Length > 0)
                path = args[0];

            RunForm(new FileExplorerForm(FileExplorerConfig.RootDirectory, path));
            return null;
        }
    }
}
