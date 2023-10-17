using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.GameResource
{
    public class LatestVersion
    {
        public LatestVersion(Model model)
        {
            NullValidator.ValidateObject(model, nameof(model));

            Release = model.release;
            Snapshot = model.snapshot;
        }

        public string Release { get; }

        public string Snapshot { get; }

        public class Model
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
            public string release { get; set; }

            public string snapshot { get; set; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        }
    }
}
