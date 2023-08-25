using Nett;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Config
{
    public class SystemConfig
    {
        public SystemConfig(Model model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));
;
            EnableAccelerationEngine = model.EnableAccelerationEngine;
            CrashAutoRestart = model.CrashAutoRestart;
            ServicesAppID = model.ServicesAppID;
            StartupChecklist = model.StartupChecklist;
        }

        public bool EnableAccelerationEngine { get; }

        public bool CrashAutoRestart { get; }

        public string ServicesAppID { get; }

        public IReadOnlyList<string> StartupChecklist { get; }

        public static SystemConfig Load(string path)
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
            if (!Validator.TryValidateObject(model, new(model), results, true))
            {
                StringBuilder message = new();
                message.AppendLine();
                int count = 0;
                foreach (var result in results)
                {
                    string memberName = result.MemberNames.FirstOrDefault() ?? string.Empty;
                    message.AppendLine($"[{memberName}]: {result.ErrorMessage}");
                    count++;
                }

                if (count > 0)
                {
                    message.Insert(0, $"解析“{name}”时遇到{count}个错误：");
                    throw new ValidationException(message.ToString().TrimEnd());
                }
            }
        }

        public class Model
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

            public bool EnableAccelerationEngine { get; set; }

            public bool CrashAutoRestart { get; set; }

            [Required(ErrorMessage = "系统服务AppID不能为空")]
            public string ServicesAppID { get; set; }

            [Required(ErrorMessage = "配置项缺失")]
            public string[] StartupChecklist { get; set; }
        }
    }
}
