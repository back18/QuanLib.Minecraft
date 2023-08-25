using Nett;
using Newtonsoft.Json.Linq;
using QuanLib.Minecraft.BlockScreen.BlockForms.SimpleFileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.SystemApplications.FileExplorer.Config
{
    public class FileExplorerConfig
    {
        public FileExplorerConfig(Model model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            RootDirectory = model.RootDirectory;
        }

        public string RootDirectory { get; }

        public static void CreateIfNotExists()
        {
            string dir = MCOS.MainDirectory.Applications.GetApplicationDirectory(FileExplorerApp.ID);
            string path = Path.Combine(dir, "Config.toml");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            CreateIfNotExists(path, "QuanLib.Minecraft.BlockScreen.SystemApplications.FileExplorer.Config.Default.Config.toml");
        }

        private static void CreateIfNotExists(string path, string resource)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException($"“{nameof(path)}”不能为 null 或空。", nameof(path));
            if (string.IsNullOrEmpty(resource))
                throw new ArgumentException($"“{nameof(resource)}”不能为 null 或空。", nameof(resource));

            if (!File.Exists(path))
            {
                using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource) ?? throw new IndexOutOfRangeException();
                using FileStream fileStream = new(path, FileMode.Create);
                stream.CopyTo(fileStream);
                Console.WriteLine($"配置文件“{path}”不存在，已创建默认配置文件");
            }
        }

        public static FileExplorerConfig Load()
        {
            string path = Path.Combine(MCOS.MainDirectory.Applications.GetApplicationDirectory(FileExplorerApp.ID), "Config.toml");
            TomlTable table = Toml.ReadFile(path);
            Model model = table.Get<Model>();

            model.RootDirectory = SimpleFilesBox.GetFullPath(model.RootDirectory);
            if (!Directory.Exists(model.RootDirectory))
                model.RootDirectory = string.Empty;

            return new(model);
        }

        public class Model
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
            public string RootDirectory { get; set; }
        }
    }
}
