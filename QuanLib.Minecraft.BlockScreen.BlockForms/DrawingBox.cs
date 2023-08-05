using FFmpeg.AutoGen;
using QuanLib.Minecraft.Block;
using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.BlockScreen.Frame;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public class DrawingBox : Control
    {
        public DrawingBox()
        {
            EnablePen = false;
            ClientSize = new(240, 110);

            _lastpos = InvalidPosition;
            _image = new(ClientSize.Width, ClientSize.Height, MinecraftResourcesManager.BlockTextureManager[BlockManager.Concrete.White].AverageColors[GetScreenPlaneSize().NormalFacing]);
            _frame = new(_image, GetScreenPlaneSize().NormalFacing, ClientSize);
            Skin.SetAllBackgroundImage(_frame);
        }

        private static readonly Point InvalidPosition = new(-1, -1);

        private Point _lastpos;

        private readonly Image<Rgba32> _image;

        private readonly ImageFrame _frame;

        public bool EnablePen { get; set; }

        public Size DrawingSize => _image.Size;

        public override IFrame RenderingFrame()
        {
            ImageFrame? image = Skin.GetBackgroundImage();
            if (image is null)
                return base.RenderingFrame();

            if (image.FrameSize != ClientSize)
            {
                image.ResizeOptions.Size = ClientSize;
                image.Update();
            }

            return image.GetFrameClone();
        }

        protected override void OnRightClick(Control sender, CursorEventArgs e)
        {
            base.OnRightClick(sender, e);

            EnablePen = !EnablePen;
        }

        protected override void OnCursorMove(Control sender, CursorEventArgs e)
        {
            base.OnCursorMove(sender, e);

            if (!EnablePen)
            {
                _lastpos = InvalidPosition;
                return;
            }

            if (_lastpos == InvalidPosition)
            {
                _lastpos = e.Position;
                return;
            }

            var item = GetScreenContext()?.Screen.InputHandler.CurrentItem;
            if (item is not null && MinecraftResourcesManager.BlockTextureManager.TryGetValue(item.ID, out var texture))
            {
                _image.Mutate(ctx =>
                {
                    var pen = new Pen(texture.AverageColors[GetScreenPlaneSize().NormalFacing], 5);
                    ctx.DrawLines(pen, new PointF[] { new(_lastpos.X, _lastpos.Y), new(e.Position.X, e.Position.Y) });
                });
                _image[e.Position.X, e.Position.Y] = texture.AverageColors[GetScreenPlaneSize().NormalFacing];
                _frame.Update();
                RequestUpdateFrame();
            }

            _lastpos = e.Position;
        }

        protected override void OnCursorEnter(Control sender, CursorEventArgs e)
        {
            base.OnCursorEnter(sender, e);

            var context = GetScreenContext();
            if (context is not null)
                context.IsShowCursor = false;
        }

        protected override void OnCursorLeave(Control sender, CursorEventArgs e)
        {
            base.OnCursorLeave(sender, e);

            var context = GetScreenContext();
            if (context is not null)
                context.IsShowCursor = true;
        }
    }
}
