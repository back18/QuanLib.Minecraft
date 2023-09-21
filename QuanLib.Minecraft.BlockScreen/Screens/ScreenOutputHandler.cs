using QuanLib.Minecraft.BlockScreen.Frame;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Screens
{
    /// <summary>
    /// 屏幕输出处理
    /// </summary>
    public class ScreenOutputHandler
    {
        public ScreenOutputHandler(Screen owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        private readonly Screen _owner;

        public ArrayFrame? LastFrame { get; internal set; }

        public bool IsGenerated => LastFrame is not null;

        public void HandleOutput(ArrayFrame frame)
        {
            List<ScreenPixel> screenPixels = GetDifferencesPixels(frame);
            List<WorldPixel> worldPixels = ToWorldPixels(screenPixels);
            LastFrame = frame;
            MCOS.Instance.ScreenManager.HandleWaitAndTasks();
            if (worldPixels.Count > 0)
            {
                MCOS.Instance.MinecraftInstance.CommandSender.OnewaySender.SendOnewayBatchSetBlock(worldPixels);
            }
        }

        public async Task HandleOutputAsync(ArrayFrame frame)
        {
            List<ScreenPixel> screenPixels = GetDifferencesPixels(frame);
            List<WorldPixel> worldPixels = ToWorldPixels(screenPixels);
            LastFrame = frame;
            MCOS.Instance.ScreenManager.HandleWaitAndTasks();
            if (worldPixels.Count > 0)
            {
                await MCOS.Instance.MinecraftInstance.CommandSender.OnewaySender.SendOnewayBatchSetBlockAsync(worldPixels);
            }
        }

        private List<ScreenPixel> GetDifferencesPixels(ArrayFrame frame)
        {
            if (frame is null)
                throw new ArgumentNullException(nameof(frame));
            if (frame.Width != _owner.Width || frame.Height != _owner.Height)
                throw new ArgumentException("帧尺寸不一致");

            List<ScreenPixel> pixels;
            if (LastFrame is null)
                pixels = frame.GetAllPixel();
            else
                pixels = ArrayFrame.GetDifferencesPixels(LastFrame, frame);

            return pixels;
        }

        private List<WorldPixel> ToWorldPixels(IEnumerable<ScreenPixel> pixels)
        {
            List<WorldPixel> worldPixels = new();
            foreach (ScreenPixel pixel in pixels)
                worldPixels.Add(_owner.ToWorldPixel(pixel));
            return worldPixels;
        }
    }
}
