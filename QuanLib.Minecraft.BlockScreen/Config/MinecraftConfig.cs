using Nett;
using Newtonsoft.Json;
using QuanLib.Minecraft;
using QuanLib.Minecraft.ResourcePack;
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

            InstanceType = model.InstanceType;
            MinecraftMode = model.MinecraftMode;
            MinecraftPath = model.MinecraftPath;
            ServerAddress = model.ServerAddress;
            JavaPath = model.JavaPath;
            LaunchArguments = model.LaunchArguments;
            McapiPort = (ushort)model.McapiPort;
            McapiPassword = model.McapiPassword;
            Language = model.Language;
            ResourcePackList = model.ResourcePackList;

            List<BlockState> list = new();
            foreach (var item in model.BlockTextureBlacklist)
            {
                if (BlockState.TryParse(item, out var blockState))
                    list.Add(blockState);
            }
            BlockTextureBlacklist = list;
        }

        public string InstanceType { get; }

        public string MinecraftMode { get; }

        public string MinecraftPath { get; }

        public string ServerAddress { get; }

        public string JavaPath { get; }

        public string LaunchArguments { get; }

        public ushort McapiPort { get; }

        public string McapiPassword { get; }

        public string Language { get; }

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

            bool isConsole = model.MinecraftMode is InstanceKeys.CONSOLE or InstanceKeys.HYBRID;
            bool isMcapi = model.MinecraftMode is InstanceKeys.MCAPI;

            if (!Validator.TryValidateObject(model, new(model), results, true))
            {
                foreach (var result in results)
                {
                    string memberName = result.MemberNames.FirstOrDefault() ?? string.Empty;
                    switch (memberName)
                    {
                        case "JavaPath":
                            if (!isConsole)
                            {
                                model.JavaPath = string.Empty;
                                continue;
                            }
                            break;
                        case "LaunchArguments":
                            if (!isConsole)
                            {
                                model.LaunchArguments = string.Empty;
                                continue;
                            }
                            break;
                        case "McapiPassword":
                            if (!isMcapi)
                            {
                                model.McapiPassword = string.Empty;
                                continue;
                            }
                            break;
                    }
                    message.AppendLine($"[{memberName}]: {result.ErrorMessage}");
                    count++;
                }
            }

            if (!(model.InstanceType is InstanceTypes.CLIENT or InstanceTypes.SERVER))
            {
                message.AppendLine($"[InstanceType]: Minecraft实例类型只能为: CLIENT, SERVER 中的其中之一");
                count++;
            }

            if (!(model.MinecraftMode is InstanceKeys.CONSOLE or InstanceKeys.HYBRID or InstanceKeys.RCON or InstanceKeys.MCAPI))
            {
                message.AppendLine($"[MinecraftMode]: Minecraft通信模式只能为 RCON, CONSOLE, HYBRID, MCAPI 中的其中之一");
                count++;
            }

            if (model.InstanceType == InstanceTypes.CLIENT && model.MinecraftMode != InstanceKeys.MCAPI)
            {
                message.AppendLine($"[MinecraftMode]: 仅支持使用MCAPI与客户端进行通信");
                count++;
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

            [Required(ErrorMessage = "Minecraft实例类型不能为空")]
            public string InstanceType { get; set; }

            [Required(ErrorMessage = "Minecraft通信模式不能为空")]
            public string MinecraftMode { get; set; }

            [Required(ErrorMessage = "Minecraft路径不能为空")]
            public string MinecraftPath { get; set; }

            [Required(ErrorMessage = "服务器IP地址不能为空")]
            public string ServerAddress { get; set; }

            [Required( ErrorMessage = "当MinecraftMode为CONSOLE或HYBRID时，Java路径不能为空")]
            public string JavaPath { get; set; }

            [Required(ErrorMessage = "当MinecraftMode为CONSOLE或HYBRID时，启动参数不能为空")]
            public string LaunchArguments { get; set; }

            [Range(0, 65535, ErrorMessage = "端口范围应该在0到65535之间")]
            public int McapiPort { get; set; }

            [Required(ErrorMessage = "语言标识不能为空")]
            public string Language { get; set; }

            [Required(ErrorMessage = "当MinecraftMode为MCAPI时，MCAPI登录密码不能为空")]
            public string McapiPassword { get; set; }

            [Required(ErrorMessage = "配置项缺失")]
            [MinLength(1, ErrorMessage = "至少需要设置一个资源包，但资源包列表为空")]
            public string[] ResourcePackList { get; set; }

            [Required(ErrorMessage = "配置项缺失")]
            public string[] BlockTextureBlacklist { get; set; }
        }
    }
}
