using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.ResourcePack
{
    public class Texture
    {
        public Texture(Image<Rgba32> image)
        {
            _image = image ?? throw new ArgumentNullException(nameof(image));
            AverageColor = GetAverageColor(image);
        }

        private readonly Image<Rgba32> _image;

        public Rgba32 AverageColor { get; }

        public Image<Rgba32> GetImage()
        {
            return _image.Clone();
        }

        private static Rgba32 GetAverageColor(Image<Rgba32> image)
        {
            int r = 0, g = 0, b = 0, a = 0;
            for (int x = 0; x < image.Width; x++)
                for (int y = 0; y < image.Height; y++)
                {
                    r += image[x, y].R;
                    g += image[x, y].G;
                    b += image[x, y].B;
                    a += image[x, y].A;
                }
            int size = image.Width * image.Height;
            r /= size;
            g /= size;
            b /= size;
            a /= size;
            return new((byte)r, (byte)g, (byte)b, (byte)a);
        }
    }
}
