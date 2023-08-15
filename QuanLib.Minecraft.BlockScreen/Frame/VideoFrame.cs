using FFMediaToolkit.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Frame
{
    public class VideoFrame : PackageFrame<Bgr24>
    {
        static VideoFrame()
        {
            DefaultResizeOptions = new()
            {
                Mode = ResizeMode.Pad,
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

        public VideoFrame(TimeSpan position, Image<Bgr24> image, Facing facing, Size size) : base(image, facing, DefaultResizeOptions.Clone())
        {
            ResizeOptions.Size = size;
            Position = position;
        }

        public VideoFrame(TimeSpan position, Image<Bgr24> image, Facing facing, ResizeOptions? resizeOptions) : base(image, facing, resizeOptions ?? DefaultResizeOptions.Clone())
        {
            Position = position;
        }

        public static ResizeOptions DefaultResizeOptions { get; }

        public TimeSpan Position { get; }

        public static Image<Bgr24> FromImageData(ReadOnlySpan<byte> data, int width, int height)
        {
            return SixLabors.ImageSharp.Image.LoadPixelData<Bgr24>(data, width, height);
        }
    }
}
