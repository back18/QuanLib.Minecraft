using QuanLib.Minecraft.BlockScreen;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Frame
{
    public class ImageFrame : PackageFrame<Rgba32>
    {
        static ImageFrame()
        {
            DefaultResizeOptions = new()
            {
                Mode = ResizeMode.Crop,
                Position = AnchorPositionMode.Center,
                CenterCoordinates = null,
                Size = new(64, 64),
                Sampler = KnownResamplers.Bicubic,
                Compand = false,
                TargetRectangle = null,
                PremultiplyAlpha = true,
                PadColor = default
            };
        }

        public ImageFrame(string path, Facing facing, ResizeOptions? resizeOptions = null) :
            this(SixLabors.ImageSharp.Image.Load<Rgba32>(File.ReadAllBytes(path)), facing, resizeOptions)
        { }

        public ImageFrame(string path, Facing facing, Size size) :
            this(SixLabors.ImageSharp.Image.Load<Rgba32>(File.ReadAllBytes(path)), facing, size)
        { }

        public ImageFrame(Image<Rgba32> image, Facing facing, Size size) : base(image, facing, DefaultResizeOptions.Clone())
        {
            ResizeOptions.Size = size;
            TransparentBlockID = string.Empty;
        }

        public ImageFrame(Image<Rgba32> image, Facing facing, ResizeOptions? resizeOptions = null) : base(image, facing, resizeOptions ?? DefaultResizeOptions.Clone())
        {
            TransparentBlockID = string.Empty;
        }

        public static ResizeOptions DefaultResizeOptions { get; }

        public string TransparentBlockID { get; set; }

        public override void Update(Rectangle? rectangle = null)
        {
            lock (Image)
            {
                _output?.Dispose();
                if (rectangle is not null)
                    _output = Image.Clone(x => x.Crop(rectangle.Value));
                else
                    _output = Image.Clone();
                _output.Mutate(x => x.Resize(ResizeOptions));
                _frame = ArrayFrame.FromImage(_facing, _output, TransparentBlockID);
                _get = false;
            }
        }

        public ImageFrame Clone()
        {
            var result = new ImageFrame(Image.Clone(), _facing, ResizeOptions.Clone())
            {
                TransparentBlockID = TransparentBlockID
            };
            return result;
        }
    }
}
