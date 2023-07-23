using QuanLib.Minecraft.BlockScreen.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.UI
{
    public class ControlRenderer
    {
        public ArrayFrame? Rendering(IControlRendering rendering)
        {
            Task<ArrayFrame> _task;
            if (rendering.NeedRendering())
            {
                if (!rendering.Visible)
                    return null;

                if (rendering.RenderingSize.Width < 0 || rendering.RenderingSize.Height < 0)
                    return null;

                _task = Task.Run(() =>
                {
                    AbstractFrame frame = rendering.RenderingFrame();
                    frame.CorrectSize(rendering.RenderingSize, rendering.ContentAnchor, rendering.Skin.GetBackgroundBlockID());
                    return frame.ToArrayFrame();
                });
                _task.ContinueWith((t) => rendering.OnRenderingCompleted(t.Result.ToArrayFrame()));
            }
            else
            {
                _task = Task.Run(() => rendering.GetFrameCache() ?? throw new InvalidOperationException());
            }

            var subRenderings = rendering.GetSubRenderings();
            if (!subRenderings.Any())
                return _task.Result.ToArrayFrame();

            List<(IControlRendering rendering, Task<ArrayFrame?> task)> results = new();
            foreach (var subRendering in subRenderings)
                results.Add((subRendering, Task.Run(() => Rendering(subRendering))));
            Task.WaitAll(results.Select(i => i.task).ToArray());
            ArrayFrame frame = _task.Result;

            foreach (var (subRendering, task) in results)
            {
                if (task.Result is null)
                    continue;

                frame.Overwrite(task.Result, subRendering.RenderingLocation);
                DrawBorder(frame, subRendering);
            }

            return frame;
        }

        private static void DrawBorder(ArrayFrame frame, IControlRendering rendering)
        {
            if (rendering.BorderWidth > 0)
            {
                int width = rendering.RenderingSize.Width + rendering.BorderWidth * 2;
                int heigth = rendering.RenderingSize.Height + rendering.BorderWidth * 2;
                int startTop = rendering.RenderingLocation.Y - 1;
                int startBottom = rendering.RenderingLocation.Y + rendering.RenderingSize.Height;
                int startLeft = rendering.RenderingLocation.X - 1;
                int startRigth = rendering.RenderingLocation.X + rendering.RenderingSize.Width;
                int endTop = rendering.RenderingLocation.Y - rendering.BorderWidth;
                int endBottom = rendering.RenderingLocation.Y + rendering.RenderingSize.Height + rendering.BorderWidth - 1;
                int endLeft = rendering.RenderingLocation.X - rendering.BorderWidth;
                int endRight = rendering.RenderingLocation.X + rendering.RenderingSize.Width + rendering.BorderWidth - 1;
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
