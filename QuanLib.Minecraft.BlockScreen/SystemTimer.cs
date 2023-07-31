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
            ScreenInput = new();
            HandleBeforeFrame = new();
            HandleAfterFrame = new();
            UIRendering = new();
            ScreenOutput = new();
            SystemInterrupt = new();
            TotalTime = new();
        }

        public Timer ProcessScheduling { get; }

        public Timer ScreenInput { get; }

        public Timer HandleBeforeFrame { get; }

        public Timer HandleAfterFrame { get; }

        public Timer UIRendering { get; }

        public Timer ScreenOutput { get; }

        public Timer SystemInterrupt { get; }

        public Timer TotalTime { get; }

        public string ToString(Timer.Duration duration)
        {
            return @$"进程调度: {Timer.FromTime(ProcessScheduling.GetTime(duration))}
屏幕输入: {Timer.FromTime(ScreenInput.GetTime(duration))}
帧前处理: {Timer.FromTime(HandleBeforeFrame.GetTime(duration))}
帧后处理: {Timer.FromTime(HandleAfterFrame.GetTime(duration))}
界面渲染: {Timer.FromTime(UIRendering.GetTime(duration))}
屏幕输出: {Timer.FromTime(ScreenOutput.GetTime(duration))}
系统中断: {Timer.FromTime(SystemInterrupt.GetTime(duration))}
共计时间: {Timer.FromTime(TotalTime.GetTime(duration))}";
        }
    }
}
