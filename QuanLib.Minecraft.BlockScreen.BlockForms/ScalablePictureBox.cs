using Newtonsoft.Json.Linq;
using QuanLib.Minecraft.Block;
using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.BlockScreen.Frame;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public class ScalablePictureBox : PictureBox
    {
        public ScalablePictureBox()
        {
            ContentAnchor = AnchorPosition.UpperLeft;

            FirstHandleCursorSlotChanged = true;
            DefaultResizeOptions.Mode = ResizeMode.Max;
            _Rectangle = new(0, 0, ImageFrame.ResizeOptions.Size.Width, ImageFrame.ResizeOptions.Size.Height);
            ScalingRatio = 0.2;
            EnableZoom = false;
            EnableDrag = false;
            Dragging = false;

            LastCursorPosition = new(0, 0);
            PixelModeThreshold = 5;
        }

        private static readonly Point InvalidPosition = new(-1, -1);

        private Point LastCursorPosition;

        public int PixelModeThreshold { get; set; }

        public bool PixelMode => GetPixelSize() >= PixelModeThreshold;

        public Rectangle Rectangle
        {
            get => _Rectangle;
            set
            {
                value = CorrectRectangle(value);

                if (_Rectangle != value)
                {
                    _Rectangle = value;
                    if (_Rectangle.Width > ClientSize.Width)
                        ImageFrame.ResizeOptions.Sampler = KnownResamplers.Bicubic;
                    else
                        ImageFrame.ResizeOptions.Sampler = KnownResamplers.NearestNeighbor;
                    ImageFrame.Update(_Rectangle);
                    if (AutoSize)
                        AutoSetSize();
                    RequestUpdateFrame();
                }
            }
        }
        private Rectangle _Rectangle;

        public double ScalingRatio { get; set; }

        public bool EnableZoom { get; set; }

        public bool EnableDrag { get; set; }

        public bool Dragging { get; private set; }

        public override IFrame RenderingFrame()
        {
            ArrayFrame frame = ImageFrame.GetFrameClone();
            int pixel = GetPixelSize();
            if (pixel >= PixelModeThreshold)
            {
                var imageFrame = ImageFrame.Clone();
                imageFrame.ResizeOptions.Size = new(Rectangle.Width * pixel, Rectangle.Height * pixel);
                imageFrame.Update(Rectangle);
                frame = imageFrame.GetFrame();
                imageFrame.Dispose();
                for (int x = frame.Width - 1; x >= 0; x -= pixel)
                    frame.FillColumn(x, BlockManager.Concrete.Gray);
                for (int y = frame.Height - 1; y >= 0; y -= pixel)
                    frame.FillRow(y, BlockManager.Concrete.Gray);
            }

            return frame;
        }

        protected override void OnCursorMove(Control sender, CursorEventArgs e)
        {
            base.OnCursorMove(sender, e);

            if (!Dragging)
            {
                LastCursorPosition = InvalidPosition;
                return;
            }
            else if (LastCursorPosition == InvalidPosition)
            {
                LastCursorPosition = e.Position;
                return;
            }

            Point position1 = ClientPos2ImagePos(LastCursorPosition);
            Point position2 = ClientPos2ImagePos(e.Position);
            Point offset = new(position2.X - position1.X, position2.Y - position1.Y);
            Rectangle rectangle = Rectangle;
            rectangle.X -= offset.X;
            rectangle.Y -= offset.Y;
            Rectangle = rectangle;
            LastCursorPosition = e.Position;
        }

        protected override void OnRightClick(Control sender, CursorEventArgs e)
        {
            base.OnRightClick(sender, e);

            if (EnableDrag)
                Dragging = !Dragging;
        }

        protected override void OnResize(Control sender, SizeChangedEventArgs e)
        {
            if (_autosetsizeing)
                return;

            Size offset = e.NewSize - e.OldSize;
            ImageFrame.ResizeOptions.Size += offset;
            DefaultResizeOptions.Size += offset;
            Rectangle = new(0, 0, ImageFrame.Image.Size.Width, ImageFrame.Image.Size.Height);
            ImageFrame.Update(Rectangle);
            if (AutoSize)
                AutoSetSize();

        }

        protected override void OnImageFrameChanged(PictureBox sender, ImageFrameChangedEventArgs e)
        {
            base.OnImageFrameChanged(sender, e);

            if (e.OldImageFrame.Image.Size != e.NewImageFrame.Image.Size)
                Rectangle = new(0, 0, e.NewImageFrame.Image.Size.Width, e.NewImageFrame.Image.Size.Height);

            if (_Rectangle.Width > ClientSize.Width)
                e.NewImageFrame.ResizeOptions.Sampler = KnownResamplers.Bicubic;
            else
                e.NewImageFrame.ResizeOptions.Sampler = KnownResamplers.NearestNeighbor;

            e.NewImageFrame.TransparentBlockID = "minecraft:glass";
        }

        protected override void OnCursorSlotChanged(Control sender, CursorSlotEventArgs e)
        {
            base.OnCursorSlotChanged(sender, e);

            if (!EnableZoom)
                return;

            Rectangle rectangle = Rectangle;

            Point position1 = ClientPos2ImagePos(rectangle, e.Position);
            Point center1 = GetImageCenter(rectangle);
            Point offset1 = new(position1.X - center1.X, position1.Y - center1.Y);

            rectangle.Width += (int)Math.Round(e.Delta * rectangle.Width * ScalingRatio);
            rectangle.Height += (int)Math.Round(e.Delta * rectangle.Height * ScalingRatio);
            rectangle.X = center1.X - (int)Math.Round(rectangle.Width / 2.0);
            rectangle.Y = center1.Y - (int)Math.Round(rectangle.Height / 2.0);

            Point position2 = ClientPos2ImagePos(rectangle, e.Position);
            Point center2 = GetImageCenter(rectangle);
            Point offset2 = new(position2.X - center2.X, position2.Y - center2.Y);

            rectangle.X += offset1.X - offset2.X;
            rectangle.Y += offset1.Y - offset2.Y;

            Rectangle = rectangle;
        }

        public Point GetImageCenter() => GetImageCenter(Rectangle);

        public Point ClientPos2ImagePos(Point position) => ClientPos2ImagePos(Rectangle, position);

        public int GetPixelSize() => GetPixelSize(Rectangle);

        private Point GetImageCenter(Rectangle rectangle)
        {
            return new(rectangle.X + (int)Math.Round(rectangle.Width / 2.0), rectangle.Y + rectangle.Height / 2);
        }

        private Point ClientPos2ImagePos(Rectangle rectangle, Point position)
        {
            int pixel = GetPixelSize(rectangle);
            if (pixel >= PixelModeThreshold)
            {
                Point pxpos = new(rectangle.X + position.X / pixel, rectangle.Y + position.Y / pixel);
                pxpos.X = Math.Min(rectangle.X + rectangle.Width - 1, pxpos.X);
                pxpos.Y = Math.Min(rectangle.Y + rectangle.Height - 1, pxpos.Y);
                return pxpos;
            }
            else
            {
                double pixels = (double)rectangle.Width / ClientSize.Width;
                return new(rectangle.X + (int)Math.Round(position.X * pixels, MidpointRounding.ToNegativeInfinity), rectangle.Y + (int)Math.Round(position.Y * pixels, MidpointRounding.ToNegativeInfinity));
            }
        }

        private int GetPixelSize(Rectangle rectangle)
        {
            if (rectangle.Width == ImageFrame.Image.Width && rectangle.Height == ImageFrame.Image.Height)
            {
                int xpx = (int)Math.Round((double)ClientSize.Width / rectangle.Width, MidpointRounding.ToNegativeInfinity);
                int ypx = (int)Math.Round((double)ClientSize.Height / rectangle.Height, MidpointRounding.ToNegativeInfinity);
                return Math.Min(xpx, ypx);
            }
            else
            {
                int xpx = (int)Math.Round((double)ClientSize.Width / rectangle.Width, MidpointRounding.ToPositiveInfinity);
                int ypx = (int)Math.Round((double)ClientSize.Height / rectangle.Height, MidpointRounding.ToPositiveInfinity);
                return Math.Max(xpx, ypx);
            }
        }

        private Rectangle CorrectRectangle(Rectangle rectangle)
        {
            if (rectangle.Width < 1)
                rectangle.Width = 1;
            else if (rectangle.Width > ImageFrame.Image.Width)
                rectangle.Width = ImageFrame.Image.Width;
            if (rectangle.Height < 1)
                rectangle.Height = 1;
            else if (rectangle.Height > ImageFrame.Image.Height)
                rectangle.Height = ImageFrame.Image.Height;

            if (rectangle.X + rectangle.Width > ImageFrame.Image.Width - 1)
                rectangle.X = ImageFrame.Image.Width - rectangle.Width;
            if (rectangle.Y + rectangle.Height > ImageFrame.Image.Height - 1)
                rectangle.Y = ImageFrame.Image.Height - rectangle.Height;

            if (rectangle.X < 0)
                rectangle.X = 0;
            else if (rectangle.X > ImageFrame.Image.Width - 1)
                rectangle.X = ImageFrame.Image.Width - 1;
            if (rectangle.Y < 0)
                rectangle.Y = 0;
            else if (rectangle.Y > ImageFrame.Image.Height - 1)
                rectangle.Y = ImageFrame.Image.Height - 1;

            return rectangle;
        }
    }
}
