using QuanLib.Minecraft.Datas;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public interface ICursorReader
    {
        /// <summary>
        /// 光标当前的操作员
        /// </summary>
        public string CurrentPlayer { get; }

        /// <summary>
        /// 光标当前的模式
        /// </summary>
        public CursorMode CurrenMode { get; }

        /// <summary>
        /// 光标当前的位置
        /// </summary>
        public Point CurrentPosition { get; }

        /// <summary>
        /// 光标当前的副手物品
        /// </summary>
        public Item? CurrentItem { get; }

        /// <summary>
        /// 当光标移动时
        /// </summary>
        public event Action<Point> OnCursorMove;

        /// <summary>
        /// 当光标右键点击时
        /// </summary>
        public event Action<Point> OnRightClick;

        /// <summary>
        /// 当光标右键点击时
        /// </summary>
        public event Action<Point> OnLeftClick;
    }
}
