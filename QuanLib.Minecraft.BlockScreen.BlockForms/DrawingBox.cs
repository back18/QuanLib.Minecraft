using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.BlockScreen.Frame;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public class DrawingBox : Control
    {
        public DrawingBox()
        {
            EnablePen = false;
            ClientSize = new(64, 64);
            DrawingSize = ClientSize;

            _framecache = ArrayFrame.BuildFrame(DrawingSize, ConcretePixel.ToBlockID(MinecraftColor.White));
        }

        private readonly ArrayFrame _framecache;

        public bool EnablePen { get; set; }

        public Size DrawingSize
        {
            get => _DrawingSize;
            set
            {
                if (_DrawingSize != value)
                {
                    _DrawingSize = value;
                    RequestUpdateFrame();
                }
            }
        }
        private Size _DrawingSize;

        public override IFrame RenderingFrame()
        {
            return _framecache.Copy();
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
                return;

            var item = GetScreenContext()?.Screen.InputHandler.CurrentItem;
            if (item is not null)
            {
                _framecache.SetBlockID(e.Position, item.ID);
                RequestUpdateFrame();
            }
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
