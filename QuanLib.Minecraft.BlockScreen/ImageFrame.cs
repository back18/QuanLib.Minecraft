using QuanLib.Minecraft.BlockScreen.Frame;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public class ImageFrame : IDisposable
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

        public ImageFrame(Image<Rgba32> image, Facing facing, Size size)
        {
            Image = image ?? throw new ArgumentNullException(nameof(image));
            _output = Image.Clone();
            _facing = facing;
            _frame = null;
            _get = false;

            ResizeOptions = DefaultResizeOptions.Clone();
            ResizeOptions.Size = size;
            TransparentBlockID = string.Empty;
        }

        public ImageFrame(Image<Rgba32> image, Facing facing, ResizeOptions? resizeOptions = null)
        {
            Image = image ?? throw new ArgumentNullException(nameof(image));
            _output = Image.Clone();
            _facing = facing;
            _frame = null;
            _get = false;

            ResizeOptions = resizeOptions ?? DefaultResizeOptions.Clone();
            TransparentBlockID = string.Empty;
        }

        public static ResizeOptions DefaultResizeOptions { get; }

        private readonly Facing _facing;

        private ArrayFrame? _frame;

        private bool _get;

        private Image<Rgba32> _output;

        public Image<Rgba32> Image { get; set; }

        public ResizeOptions ResizeOptions { get; }

        public string TransparentBlockID { get; set; }

        public Size FrameSize
        {
            get
            {
                if (_frame is null)
                    Update();
                return new(_frame!.Width, _frame!.Height);
            }
        }

        public ArrayFrame GetFrame()
        {
            _get = true;
            if (_frame is null)
                Update();
            return _frame!;
        }

        public ArrayFrame GetFrameClone()
        {
            if (_get)
                return GetFrame().Clone();
            else
                return GetFrame();
        }

        public void Update(Rectangle? rectangle = null)
        {
            _output.Dispose();
            if (rectangle is not null)
                _output = Image.Clone(x => x.Crop(rectangle.Value));
            else
                _output = Image.Clone();
            _output.Mutate(x => x.Resize(ResizeOptions));
            _frame = ArrayFrame.FromImage(_facing, _output, TransparentBlockID);
            _get = false;
        }

        public ImageFrame Clone()
        {
            var result = new ImageFrame(Image.Clone(), _facing, ResizeOptions.Clone())
            {
                TransparentBlockID = TransparentBlockID
            };
            return result;
        }

        public void Dispose()
        {
            Image.Dispose();
            _output.Dispose();

            GC.SuppressFinalize(this);
        }

        public static bool operator ==(ImageFrame? image1, ImageFrame? image2)
        {
            return image1?.Image == image2?.Image;
        }

        public static bool operator !=(ImageFrame? image1, ImageFrame? image2)
        {
            return image1?.Image != image2?.Image;
        }
    }
}
