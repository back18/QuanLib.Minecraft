using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public class SystemTimer
    {
        public SystemTimer()
        {
            ProcessScheduling = new();
            CursorEvent = new();
            HandleBeforeFrame = new();
            HandleAfterFrame = new();
            RenderingFrame = new();
            UpdateScreen = new();
            SystemInterrupt = new();
            TotalTime = new();
        }

        public Timer ProcessScheduling { get; }

        public Timer CursorEvent { get; }

        public Timer HandleBeforeFrame { get; }

        public Timer HandleAfterFrame { get; }

        public Timer RenderingFrame { get; }

        public Timer UpdateScreen { get; }

        public Timer SystemInterrupt { get; }

        public Timer TotalTime { get; }

        public string ToString(Timer.Duration duration)
        {
            return @$"进程调度: {Timer.FromTime(ProcessScheduling.GetTime(duration))}
光标事件: {Timer.FromTime(CursorEvent.GetTime(duration))}
帧前处理: {Timer.FromTime(HandleBeforeFrame.GetTime(duration))}
帧后处理: {Timer.FromTime(HandleAfterFrame.GetTime(duration))}
帧渲染器: {Timer.FromTime(RenderingFrame.GetTime(duration))}
更新屏幕: {Timer.FromTime(UpdateScreen.GetTime(duration))}
系统中断: {Timer.FromTime(SystemInterrupt.GetTime(duration))}
共计时间: {Timer.FromTime(TotalTime.GetTime(duration))}";
        }
    }
}
