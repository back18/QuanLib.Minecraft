using QuanLib.Minecraft.BlockScreen.Screens;
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
            ScreenScheduling = new();
            ProcessScheduling = new();
            FormScheduling = new();
            InteractionScheduling = new();
            ScreenInput = new();
            HandleBeforeFrame = new();
            UIRendering = new();
            ScreenOutput = new();
            HandleAfterFrame = new();
            ScreenBuild = new();
            SystemInterrupt = new();
            TotalTime = new();
        }

        public Timer ScreenScheduling { get; }

        public Timer ProcessScheduling { get; }

        public Timer FormScheduling { get; }

        public Timer InteractionScheduling { get; }

        public Timer ScreenInput { get; }

        public Timer HandleBeforeFrame { get; }

        public Timer UIRendering { get; }

        public Timer ScreenOutput { get; }

        public Timer HandleAfterFrame { get; }

        public Timer ScreenBuild { get; }

        public Timer SystemInterrupt { get; }

        public Timer TotalTime { get; }

        public string ToString(Timer.Duration duration)
        {
            return @$"屏幕调度: {Timer.FromTime(ScreenScheduling.GetTime(duration))}
进程调度: {Timer.FromTime(ProcessScheduling.GetTime(duration))}
窗体调度: {Timer.FromTime(FormScheduling.GetTime(duration))}
交互调度: {Timer.FromTime(InteractionScheduling.GetTime(duration))}
屏幕输入: {Timer.FromTime(ScreenInput.GetTime(duration))}
帧前处理: {Timer.FromTime(HandleBeforeFrame.GetTime(duration))}
界面渲染: {Timer.FromTime(UIRendering.GetTime(duration))}
屏幕输出: {Timer.FromTime(ScreenOutput.GetTime(duration))}
帧后处理: {Timer.FromTime(HandleAfterFrame.GetTime(duration))}
屏幕构建: {Timer.FromTime(ScreenBuild.GetTime(duration))}
系统中断: {Timer.FromTime(SystemInterrupt.GetTime(duration))}
共计时间: {Timer.FromTime(TotalTime.GetTime(duration))}";
        }
    }
}
