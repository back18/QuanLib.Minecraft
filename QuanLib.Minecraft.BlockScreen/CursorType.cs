using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public enum CursorType
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default,

        /// <summary>
        /// 编辑
        /// </summary>
        Edit,

        /// <summary>
        /// 按
        /// </summary>
        Press,

        /// <summary>
        /// 等待
        /// </summary>
        Wait,

        /// <summary>
        /// 水平调整尺寸
        /// </summary>
        HorizontalResize,

        /// <summary>
        /// 垂直调整尺寸
        /// </summary>
        VerticalResize,

        /// <summary>
        /// 左斜调整尺寸
        /// </summary>
        LeftObliqueResize,

        /// <summary>
        /// 右斜调整尺寸
        /// </summary>
        RightObliqueResize,
    }
}
