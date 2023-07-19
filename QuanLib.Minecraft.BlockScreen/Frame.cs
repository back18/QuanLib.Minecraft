using Newtonsoft.Json;
using QuanLib.Minecraft.Files;
using QuanLib.Minecraft.BlockScreen.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace QuanLib.Minecraft.BlockScreen
{
    public class Frame
    {
        public Frame(string[,] ids)
        {
            _ids = ids ?? throw new ArgumentNullException(nameof(ids));
        }

        public readonly string[,] _ids;
        
        public int Width => _ids.GetLength(0);

        public int Height => _ids.GetLength(1);

        public string GetBlockID(int x, int y)
            => _ids[x, y];

        public void SetBlockID(int x, int y, string value)
            => _ids[x, y] = value;

        public string[,] GetAllBlockID()
            => _ids;

        public Frame Copy()
        {
            string[,] copy = ArrayUtil.Copy(_ids);
            return new(copy);
        }

        public void Overwrite(Frame frame, Point location, Point offset)
        {
            if (frame is null)
                throw new ArgumentNullException(nameof(frame));

            int offsetX = location.X - offset.X;
            int offsetY = location.Y - offset.Y;

            int startX1, startY1, startX2, startY2;
            if (offsetX < 0)
            {
                startX1 = 0;
                startX2 = -offsetX;
            }
            else
            {
                startX1 = offsetX;
                startX2 = 0;
            }
            if (offsetY < 0)
            {
                startY1 = 0;
                startY2 = -offsetY;
            }
            else
            {
                startY1 = offsetY;
                startY2 = 0;
            }
            int maxWidth = offsetX + frame.Width < Width ? offsetX + frame.Width : Width;
            int maxHeight = offsetY + frame.Height < Height ? offsetY + frame.Height : Height;
            for (int x1 = startX1, x2 = startX2; x1 < maxWidth; x1++, x2++)
                for (int y1 = startY1, y2 = startY2; y1 < maxHeight; y1++, y2++)
                {
                    string newBlcokID = frame.GetBlockID(x2, y2);
                    if (!string.IsNullOrEmpty(newBlcokID))
                        _ids[x1, y1] = newBlcokID;
                }
        }

        public List<ScreenPixel> GetAllPixel()
        {
            int width = Width;
            int height = Height;
            List<ScreenPixel> result = new(width * height);
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    result.Add(new(new(x, y), _ids[x, y]));
            return result;
        }

        public static List<ScreenPixel> GetDifferencesPixels(Frame oldFrame, Frame newFrame)
        {
            if (oldFrame is null)
                throw new ArgumentNullException(nameof(oldFrame));

            if (newFrame is null)
                throw new ArgumentNullException(nameof(newFrame));

            if (oldFrame.Width != newFrame.Width || oldFrame.Height != newFrame.Height)
                throw new ArgumentException("参数必须为相同尺寸的帧");

            List<ScreenPixel> result = new();
            int width = newFrame.Width;
            int height = newFrame.Height;
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    string newBlockID = newFrame.GetBlockID(x, y);
                    if (newBlockID != oldFrame.GetBlockID(x, y))
                        result.Add(new(new(x, y), newBlockID));
                }

            return result;
        }

        public static Frame BuildFrame(int width, int height, string id)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));
            return new(ArrayUtil.FillToNewArray(width, height, id));
        }

        public static Frame BuildFrame(bool[,] bits, string foregroundBlockID, string backgroundBlockID)
        {
            if (bits is null)
                throw new ArgumentNullException(nameof(bits));
            if (foregroundBlockID is null)
                throw new ArgumentNullException(nameof(foregroundBlockID));
            if (backgroundBlockID is null)
                throw new ArgumentNullException(nameof(backgroundBlockID));

            int width = bits.GetLength(0);
            int height = bits.GetLength(1);
            string[,] datas = new string[width, height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    if (bits[x, y])
                        datas[x, y] = foregroundBlockID;
                    else
                        datas[x, y] = backgroundBlockID;
                }

            return new Frame(datas);
        }

        public static Frame FromJson(FrameJson json)
        {
            if (json is null)
                throw new ArgumentNullException(nameof(json));

            try
            {
                string[,] ids = new string[json.Width, json.Height];
                for (int x = 0; x < json.Width; x++)
                    for (int y = 0; y < json.Height; y++)
                        ids[x, y] = json.Map[json.Data[y][x]];
                return new Frame(ids);
            }
            catch (Exception ex)
            {
                throw new FormatException("无法解析 FrameJson 对象", ex);
            }
        }

        public static Frame FromJson(string json)
        {
            return FromJson(JsonConvert.DeserializeObject<FrameJson>(json) ?? throw new FormatException());
        }

        public static Frame FromImage<T>(Facing facing, Image<T> image) where T : unmanaged, IPixel<T>
        {
            string[,] ids = new string[image.Width, image.Height];
            for (int x = 0; x < image.Width; x++)
                for (int y = 0; y < image.Height; y++)
                    ids[x, y] = MCOS.BlockTextureCollection.MatchBlockTexture(facing, image[x, y])?.BlockID ?? "minecraft:air";
            return new(ids);
        }

        public static Frame FromImage(Facing facing, string path)
        {
            using var image = Image.Load<Rgba32>(File.ReadAllBytes(path));
            return FromImage(facing, image);
        }
    }
}
