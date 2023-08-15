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
    public abstract class PackageFrame<T> : IDisposable where T : unmanaged, IPixel<T>
    {
        protected PackageFrame(Image<T> image, Facing facing, ResizeOptions resizeOptions = null)
        {
            Image = image ?? throw new ArgumentNullException(nameof(image));
            ResizeOptions = resizeOptions ?? throw new ArgumentNullException(nameof(image));

            _facing = facing;
            _output = null;
            _frame = null;
            _get = false;
        }

        protected bool _get;

        protected readonly Facing _facing;

        protected ArrayFrame? _frame;

        protected Image<T>? _output;

        public Image<T> Image { get; set; }

        public ResizeOptions ResizeOptions { get; }

        public Size FrameSize
        {
            get
            {
                if (_frame is null)
                    Update();
                return new(_frame!.Width, _frame!.Height);
            }
        }

        public virtual ArrayFrame GetFrame()
        {
            _get = true;
            if (_frame is null)
                Update();
            return _frame!;
        }

        public virtual ArrayFrame GetFrameClone()
        {
            if (_get)
                return GetFrame().Clone();
            else
                return GetFrame();
        }

        public virtual void Update(Rectangle? rectangle = null)
        {
            lock (Image)
            {
                _output?.Dispose();
                if (rectangle is not null)
                    _output = Image.Clone(x => x.Crop(rectangle.Value));
                else
                    _output = Image.Clone();
                _output.Mutate(x => x.Resize(ResizeOptions));
                _frame = ArrayFrame.FromImage(_facing, _output);
                _get = false;
            }
        }

        public virtual void Dispose()
        {
            _output?.Dispose();
            Image.Dispose();

            GC.SuppressFinalize(this);
        }

        public static bool operator ==(PackageFrame<T>? image1, PackageFrame<T>? image2)
        {
            return image1?.Image == image2?.Image;
        }

        public static bool operator !=(PackageFrame<T>? image1, PackageFrame<T>? image2)
        {
            return image1?.Image != image2?.Image;
        }
    }
}
