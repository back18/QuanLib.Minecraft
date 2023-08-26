using Nett;
using Newtonsoft.Json;
using QuanLib.Minecraft;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Config
{
    public class MinecraftConfig
    {
        private MinecraftConfig(Model model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            ServerMode = (MinecraftServerMode)model.ServerMode;
            ServerPath = model.ServerPath;
            ServerAddress = model.ServerAddress;
            JavaPath = model.JavaPath;
            LaunchArguments = model.LaunchArguments;
            AccelerationEngineEventPort = (ushort)model.AccelerationEngineEventPort;
            AccelerationEngineDataPort = (ushort)model.AccelerationEngineDataPort;
            ResourcePackList = model.ResourcePackList;

            List<BlockState> list = new();
            foreach (var item in model.BlockTextureBlacklist)
            {
                if (BlockState.TryParse(item, out var blockState))
                    list.Add(blockState);
            }
            BlockTextureBlacklist = list;
        }

        public MinecraftServerMode ServerMode { get; }

        public string ServerPath { get; }

        public string ServerAddress { get; }

        public string JavaPath { get; }

        public string LaunchArguments { get; }

        public ushort AccelerationEngineEventPort { get; }

        public ushort AccelerationEngineDataPort { get; }

        public IReadOnlyList<string> ResourcePackList { get; }

        public IReadOnlyList<BlockState> BlockTextureBlacklist { get; }

        public static MinecraftConfig Load(string path)
        {
            TomlTable table = Toml.ReadFile(path);
            Model model = table.Get<Model>();
            Validate(Path.GetFileName(path), model);
            return new(model);
        }

        public static void Validate(string name, Model model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            List<ValidationResult> results = new();
            StringBuilder message = new();
            message.AppendLine();
            int count = 0;

            if (!Validator.TryValidateObject(model, new(model), results, true))
            {
                foreach (var result in results)
                {
                    string memberName = result.MemberNames.FirstOrDefault() ?? string.Empty;
                    switch (memberName)
                    {
                        case "JavaPath":
                            if (model.ServerMode != 1)
                            {
                                model.JavaPath = string.Empty;
                                continue;
                            }
                            break;
                        case "LaunchArguments":
                            if (model.ServerMode != 1)
                            {
                                model.LaunchArguments = string.Empty;
                                continue;
                            }
                            break;
                        default:
                            break;
                    }
                    message.AppendLine($"[{memberName}]: {result.ErrorMessage}");
                    count++;
                }
            }

            foreach (string resourcePack in model.ResourcePackList)
            {
                if (!MCOS.MainDirectory.MinecraftResources.ResourcePacks.ExistsFile(resourcePack))
                {
                    message.AppendLine($"[ResourcePackList]: 资源包“{resourcePack}”不存在");
                    count++;
                }
            }

            if (count > 0)
            {
                message.Insert(0, $"解析“{name}”时遇到{count}个错误：");
                throw new ValidationException(message.ToString().TrimEnd());
            }
        }

        public class Model
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

            [Range(0, 1, ErrorMessage = "服务器模式应该为0或1")]
            public int ServerMode { get; set; }

            [Required(ErrorMessage = "服务端路径不能为空")]
            public string ServerPath { get; set; }

            [Required(ErrorMessage = "服务器地址不能为空")]
            public string ServerAddress { get; set; }

            [Required( ErrorMessage = "当ServerMode为1时，Java路径不能为空")]
            public string JavaPath { get; set; }

            [Required(ErrorMessage = "当ServerMode为1时，启动参数不能为空")]
            public string LaunchArguments { get; set; }

            [Range(0, 65535, ErrorMessage = "端口范围应该在0到65535之间")]
            public int AccelerationEngineEventPort { get; set; }

            [Range(0, 65535, ErrorMessage = "端口范围应该在0到65535之间")]
            public int AccelerationEngineDataPort { get; set; }

            [Required(ErrorMessage = "配置项缺失")]
            [MinLength(1, ErrorMessage = "至少选择一个资源包，但资源包列表为空")]
            public string[] ResourcePackList { get; set; }

            [Required(ErrorMessage = "配置项缺失")]
            public string[] BlockTextureBlacklist { get; set; }
        }
    }
}
