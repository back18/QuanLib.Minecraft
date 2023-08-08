using QuanLib.Minecraft.BlockScreen.Frame;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.UI
{
    public static class UIRenderer
    {
        public static ArrayFrame? Rendering(IControl control)
        {
            IControlRendering rendering = control;
            Task<ArrayFrame> _task;
            if (rendering.NeedRendering())
            {
                if (!rendering.Visible)
                    return null;

                if (rendering.ClientSize.Width < 0 || rendering.ClientSize.Height < 0)
                    return null;

                _task = Task.Run(() =>
                {
                    IFrame frame = rendering.RenderingFrame();

                    frame.CorrectSize(rendering.ClientSize, rendering.OffsetPosition, rendering.ContentAnchor, rendering.Skin.GetBackgroundBlockID());
                    return frame.ToArrayFrame();
                });
                _task.ContinueWith((t) => rendering.HandleRenderingCompleted(new(t.Result.ToArrayFrame())));
            }
            else
            {
                _task = Task.Run(() => rendering.GetFrameCache() ?? throw new InvalidOperationException("无法获取帧缓存"));
            }

            var subControls = (control as IContainerControl)?.GetSubControls();
            if (subControls is null || !subControls.Any())
                return _task.Result.ToArrayFrame();

            List<(IControlRendering rendering, Task<ArrayFrame?> task)> tasks = new();
            foreach (var subControl in subControls)
                tasks.Add((subControl, Task.Run(() => Rendering(subControl))));
            Task.WaitAll(tasks.Select(i => i.task).ToArray());
            ArrayFrame frame = _task.Result;

            foreach (var (subControl, task) in tasks)
            {
                if (task.Result is null)
                    continue;

                frame.Overwrite(task.Result, subControl.GetRenderingLocation(), rendering.OffsetPosition);
                DrawBorder(frame, subControl, rendering.OffsetPosition);
            }

            return frame;
        }

        private static void DrawBorder(ArrayFrame frame, IControlRendering rendering, Point offset)
        {
            if (rendering.BorderWidth > 0)
            {
                int width = rendering.ClientSize.Width + rendering.BorderWidth * 2;
                int heigth = rendering.ClientSize.Height + rendering.BorderWidth * 2;

                Point location = rendering.GetRenderingLocation();
                location = new(location.X - offset.X, location.Y - offset.Y);
                int startTop = location.Y - 1;
                int startBottom = location.Y + rendering.ClientSize.Height;
                int startLeft = location.X - 1;
                int startRigth = location.X + rendering.ClientSize.Width;
                int endTop = location.Y - rendering.BorderWidth;
                int endBottom = location.Y + rendering.ClientSize.Height + rendering.BorderWidth - 1;
                int endLeft = location.X - rendering.BorderWidth;
                int endRight = location.X + rendering.ClientSize.Width + rendering.BorderWidth - 1;

                string blockID = rendering.Skin.GetBorderBlockID();

                for (int y = startTop; y >= endTop; y--)
                    frame.DrawRow(y, endLeft, width, blockID);
                for (int y = startBottom; y <= endBottom; y++)
                    frame.DrawRow(y, endLeft, width, blockID);
                for (int x = startLeft; x >= endLeft; x--)
                    frame.DrawColumn(x, endTop, heigth, blockID);
                for (int x = startRigth; x <= endRight; x++)
                    frame.DrawColumn(x, endTop, heigth, blockID);
            }
        }
    }
}
