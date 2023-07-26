using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Data
{
    public class FrameJson
    {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public int Width { get; set; }

        public int Height { get; set; }

        public Dictionary<char, string> Map { get; set; }

        public List<string> Data;
    }
}
