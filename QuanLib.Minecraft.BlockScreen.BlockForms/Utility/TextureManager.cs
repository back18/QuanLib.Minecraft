using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.BlockForms.Utility
{
    public static class TextureManager
    {
        private static readonly Dictionary<string, Image<Rgba32>> _items = new();

        public static void Load(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException();

            _items.Clear();
            string[] files = Directory.GetFiles(path, "*.png");
            foreach (string file in files)
            {
                try
                {
                    _items.Add(Path.GetFileNameWithoutExtension(file), Image.Load<Rgba32>(File.ReadAllBytes(Path.Combine(file))));
                }
                catch
                {

                }
            }
        }

        public static Image<Rgba32> GetTexture(string key)
        {
            return _items[key];
        }

        public static bool TryGetTexture(string key, [MaybeNullWhen(false)] out Image<Rgba32> texture)
        {
            return _items.TryGetValue(key, out texture);
        }
    }
}
