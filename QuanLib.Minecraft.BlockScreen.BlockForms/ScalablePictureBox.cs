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
            FirstHandleCursorSlotChanged = true;
            DefaultResizeOptions.Mode = ResizeMode.Max;
            AutoSize = true;
            Rectangle = new(0, 0, ImageFrame.ResizeOptions.Size.Width, ImageFrame.ResizeOptions.Size.Height);

            _setsizeing = false;
        }

        private bool _setsizeing;

        public Rectangle Rectangle
        {
            get => _Rectangle;
            set
            {
                if (value.Width < 1)
                    value.Width = 1;
                else if (value.X + value.Width > ImageFrame.Image.Width - 1)
                {
                    if (value.Width > ImageFrame.Image.Width)
                        value.Width = ImageFrame.Image.Width;
                    value.X = ImageFrame.Image.Width - value.Width;
                }
                if (value.Height < 1)
                    value.Height = 1;
                else if (value.Y + value.Height > ImageFrame.Image.Height - 1)
                {
                    if (value.Height > ImageFrame.Image.Height)
                        value.Height = ImageFrame.Image.Height;
                    value.Y = ImageFrame.Image.Height - value.Height;
                }

                if (value.X < 0)
                    value.X = 0;
                else if (value.X > ImageFrame.Image.Width - 1)
                    value.X = ImageFrame.Image.Width - 1;
                if (value.Y < 0)
                    value.Y = 0;
                else if (value.Y > ImageFrame.Image.Height - 1)
                    value.Y = ImageFrame.Image.Height - 1;

                if (_Rectangle != value)
                {
                    _Rectangle = value;
                    ImageFrame.Update(_Rectangle);
                    AutoSetSize();
                    RequestUpdateFrame();
                }
            }
        }
        private Rectangle _Rectangle;

        public override IFrame RenderingFrame()
        {
            return ImageFrame.GetFrameClone();
        }

        protected override void OnResize(Control sender, SizeChangedEventArgs e)
        {
            base.OnResize(sender, e);

            if (_setsizeing)
                return;

            Size offset = e.NewSize - e.OldSize;
            ImageFrame.ResizeOptions.Size += offset;
            ImageFrame.Update();
            Rectangle = new(0, 0, ImageFrame.Image.Size.Width, ImageFrame.Image.Size.Height);
        }

        protected override void OnImageFrameChanged(PictureBox sender, ImageFrameChangedEventArgs e)
        {
            base.OnImageFrameChanged(sender, e);

            Rectangle = new(0, 0, e.NewImageFrame.Image.Size.Width, e.NewImageFrame.Image.Size.Height);
        }

        protected override void OnCursorSlotChanged(Control sender, CursorSlotEventArgs e)
        {
            base.OnCursorSlotChanged(sender, e);

            Rectangle rectangle = Rectangle;
            double pixels = rectangle.Width / ClientSize.Width;
            rectangle.Width += e.Delta * (rectangle.Width / 10);
            rectangle.Height += e.Delta * (rectangle.Height / 10);
            rectangle.X = (int)Math.Round(e.Position.X * pixels - rectangle.Width / pixels / 2);
            rectangle.Y = (int)Math.Round(e.Position.Y * pixels - rectangle.Height / pixels / 2);
            Rectangle = rectangle;
            Console.WriteLine(Rectangle.ToString());
        }

        public override void AutoSetSize()
        {
            _setsizeing = true;
            ClientSize = ImageFrame.FrameSize;
            _setsizeing = false;
        }
    }
}
