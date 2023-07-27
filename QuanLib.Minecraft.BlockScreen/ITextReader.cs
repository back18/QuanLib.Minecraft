using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public interface ITextReader
    {
        /// <summary>
        /// 文本编辑器的初始文本
        /// </summary>
        public string InitialText { get; set; }

        /// <summary>
        /// 文本编辑器当前的文本
        /// </summary>
        public string CurrentText { get; }

        /// <summary>
        /// 当文本编辑器更新时
        /// </summary>
        public event Action<Point, string> OnTextUpdate;
    }
}
