using FFMediaToolkit.Decoding;
using FFMediaToolkit.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QuanLib.Minecraft.BlockScreen
{
    public static class Extensions
    {
        public static Image<TPixel> ToImage<TPixel>(this ImageData source) where TPixel : unmanaged, IPixel<TPixel>
        {
            return Image.LoadPixelData<TPixel>(source.Data, source.ImageSize.Width, source.ImageSize.Height);
        }

        public static MediaOptions Copy(this MediaOptions source)
        {
            return new()
            {
                PacketBufferSizeLimit = source.PacketBufferSizeLimit,
                DemuxerOptions = source.DemuxerOptions,
                VideoPixelFormat = source.VideoPixelFormat,
                TargetVideoSize = source.TargetVideoSize,
                VideoSeekThreshold = source.VideoSeekThreshold,
                AudioSeekThreshold = source.AudioSeekThreshold,
                DecoderThreads = source.DecoderThreads,
                DecoderOptions = source.DecoderOptions,
                StreamsToLoad = source.StreamsToLoad
            };
        }

        public static ResizeOptions Clone(this ResizeOptions source)
        {
            return new()
            {
                Mode = source.Mode,
                Position = source.Position,
                CenterCoordinates = source.CenterCoordinates,
                Size = source.Size,
                Sampler = source.Sampler,
                Compand = source.Compand,
                TargetRectangle = source.TargetRectangle,
                PremultiplyAlpha = source.PremultiplyAlpha,
                PadColor = source.PadColor
            };
        }
    }
}
