using QuanLib.Minecraft.Block;
using QuanLib.Minecraft.BlockScreen.Event;
using QuanLib.Minecraft.BlockScreen.Frame;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms
{
    public class PictureBox : Control
    {
        public PictureBox()
        {
            DefaultResizeOptions = ImageFrame.DefaultResizeOptions.Clone();
            _ImageFrame = new(new Image<Rgba32>(DefaultResizeOptions.Size.Width, DefaultResizeOptions.Size.Height, GetBlockAverageColor(BlockManager.Concrete.White)), GetScreenPlaneSize().NormalFacing);
            ClientSize = DefaultResizeOptions.Size;

            AutoSize = true;
            ContentAnchor = AnchorPosition.Centered;

            ImageFrameChanged += OnImageFrameChanged;
        }

        public ResizeOptions DefaultResizeOptions { get; }

        public ImageFrame ImageFrame
        {
            get => _ImageFrame;
            set
            {
                if (_ImageFrame != value)
                {
                    ImageFrame temp = _ImageFrame;
                    _ImageFrame = value;
                    ImageFrameChanged.Invoke(this, new(temp, _ImageFrame));
                    RequestUpdateFrame();
                }
            }
        }
        private ImageFrame _ImageFrame;

        public event EventHandler<PictureBox, ImageFrameChangedEventArgs> ImageFrameChanged;

        protected virtual void OnImageFrameChanged(PictureBox sender, ImageFrameChangedEventArgs e)
        {
            e.OldImageFrame.Dispose();
            if (AutoSize)
                AutoSetSize();
        }

        public override IFrame RenderingFrame()
        {
            if (ImageFrame.FrameSize != ClientSize)
            {
                ImageFrame.ResizeOptions.Size = ClientSize;
                ImageFrame.Update();
            }

            return ImageFrame.GetFrameClone();
        }

        public override void AutoSetSize()
        {
            ClientSize = ImageFrame.FrameSize;
        }

        public void SetImage(Image<Rgba32> image)
        {
            ImageFrame = new(image, GetScreenPlaneSize().NormalFacing, DefaultResizeOptions);
        }

        public bool TryReadImageFile(string path)
        {
            if (!File.Exists(path))
                return false;

            try
            {
                ImageFrame = new(Image.Load<Rgba32>(File.ReadAllBytes(path)), GetScreenPlaneSize().NormalFacing, DefaultResizeOptions);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
