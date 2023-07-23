using QuanLib.Minecraft.BlockScreen.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.UI
{
    public class ControlRenderer : UIRenderer<IControlRendering>
    {
        public override ArrayFrame? Rendering(IControlRendering rendering)
        {
            Task<ArrayFrame> _task;
            if (rendering.NeedRendering())
            {
                if (!rendering.Visible)
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
                if (subRendering.BorderWidth > 0)
                {
                    int width = subRendering.RenderingSize.Width + subRendering.BorderWidth * 2;
                    int heigth = subRendering.RenderingSize.Height + subRendering.BorderWidth * 2;
                    int startTop = subRendering.RenderingLocation.Y - 1;
                    int startBottom = subRendering.RenderingLocation.Y + subRendering.RenderingSize.Height;
                    int startLeft = subRendering.RenderingLocation.X - 1;
                    int startRigth = subRendering.RenderingLocation.X + subRendering.RenderingSize.Width;
                    int endTop = subRendering.RenderingLocation.Y - subRendering.BorderWidth;
                    int endBottom = subRendering.RenderingLocation.Y + subRendering.RenderingSize.Height + subRendering.BorderWidth - 1;
                    int endLeft = subRendering.RenderingLocation.X - subRendering.BorderWidth;
                    int endRight = subRendering.RenderingLocation.X + subRendering.RenderingSize.Width + subRendering.BorderWidth - 1;
                    string blockID = subRendering.Skin.GetBorderBlockID();

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

            return frame;
        }
    }
}
