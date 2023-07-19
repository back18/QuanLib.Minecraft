using FFMediaToolkit.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public class VideoFrame
    {
        private VideoFrame(TimeSpan position, Image<Bgr24> image)
        {
            Position = position;
            Image = image;
        }

        public TimeSpan Position { get; }

        public Image<Bgr24> Image { get; }

        public static VideoFrame FromImageData(TimeSpan position, ReadOnlySpan<byte> data, int width, int height)
        {
            return new(position, SixLabors.ImageSharp.Image.LoadPixelData<Bgr24>(data, width, height));
        }
    }
}
