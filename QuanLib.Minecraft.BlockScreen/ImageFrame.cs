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
                Size = new(64, 64),
                Mode = ResizeMode.Pad,
                Position = AnchorPositionMode.Center,
                Sampler = KnownResamplers.Bicubic
            };
        }

        public ImageFrame(string path, Facing facing, ResizeOptions? resizeOptions = null) :
            this(Image.Load<Rgba32>(File.ReadAllBytes(path)), facing, resizeOptions)
        { }

        public ImageFrame(string path, Facing facing, Size size) :
            this(Image.Load<Rgba32>(File.ReadAllBytes(path)), facing, size)
        { }

        public ImageFrame(Image<Rgba32> image, Facing facing, Size size) :
            this(image, facing, new ResizeOptions()
            {
                Size = size,
                Mode = DefaultResizeOptions.Mode,
                Position = DefaultResizeOptions.Position,
                Sampler = DefaultResizeOptions.Sampler
            })
        { }

        public ImageFrame(Image<Rgba32> image, Facing facing, ResizeOptions? resizeOptions = null)
        {
            _original = image ?? throw new ArgumentNullException(nameof(image));
            _facing = facing;
            _frame = null;
            _get = false;

            ResizeOptions = resizeOptions ?? new()
            {
                Size = DefaultResizeOptions.Size,
                Mode = DefaultResizeOptions.Mode,
                Position = DefaultResizeOptions.Position,
                Sampler = DefaultResizeOptions.Sampler
            };
        }

        public static ResizeOptions DefaultResizeOptions { get; }

        private readonly Facing _facing;

        private readonly Image<Rgba32> _original;

        private Image<Rgba32>? _result;

        private ArrayFrame? _frame;

        private bool _get;

        public ResizeOptions ResizeOptions { get; }

        public Size FrameSize
        {
            get
            {
                ArrayFrame frame = _frame ?? Update();
                return new(frame.Width, frame.Height);
            }
        }

        public ArrayFrame GetFrame()
        {
            _get = true;
            return _frame ?? Update();
        }

        public ArrayFrame GetFrameCopy()
        {
            if (_get)
                return GetFrame().Copy();
            else
                return GetFrame();
        }

        public ArrayFrame Update()
        {
            _result?.Dispose();
            _result = _original.Clone();
            _result.Mutate(x => x.Resize(ResizeOptions));
            _frame = ArrayFrame.FromImage(_facing, _result);
            return _frame;
        }

        public void Dispose()
        {
            _original.Dispose();
            _result?.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
