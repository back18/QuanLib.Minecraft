using QuanLib.Core;
using QuanLib.Minecraft.BlockScreen.Event;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms.Utility
{
    /// <summary>
    /// 控件布局同步器
    /// </summary>
    public class LayoutSyncer
    {
        public LayoutSyncer(Control target, EventHandler<Control, PositionChangedEventArgs> move, EventHandler<Control, SizeChangedEventArgs> resize)
        {
            Target = target ?? throw new ArgumentNullException(nameof(target));
            Move = move;
            Resize = resize;
        }

        public Control Target { get; }

        public event EventHandler<Control, PositionChangedEventArgs> Move;

        public event EventHandler<Control, SizeChangedEventArgs> Resize;

        /// <summary>
        /// 绑定
        /// </summary>
        public void Binding()
        {
            Target.Move += Move;
            Target.Resize += Resize;
        }

        /// <summary>
        /// 解绑
        /// </summary>
        public void Unbinding()
        {
            Target.Move -= Move;
            Target.Resize -= Resize;
        }

        /// <summary>
        /// 主动调用同步委托
        /// </summary>
        public void Sync()
        {
            Move.Invoke(Target, new(Target.ClientLocation, Target.ClientLocation));
            Resize.Invoke(Target, new(Target.ClientSize, Target.ClientSize));
        }
    }
}
