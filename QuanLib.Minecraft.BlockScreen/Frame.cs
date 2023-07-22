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
using QuanLib.ExceptionHelpe;

namespace QuanLib.Minecraft.BlockScreen
{
    public class Frame
    {
        public Frame(string[,] ids)
        {
            _ids = ids ?? throw new ArgumentNullException(nameof(ids));
        }

        public Frame(Frame frame)
        {
            if (frame is null)
                throw new ArgumentNullException(nameof(frame));

            _ids = frame._ids;
        }

        private string[,] _ids;

        public int Width => _ids.GetLength(0);

        public int Height => _ids.GetLength(1);

        public string GetBlockID(int x, int y)
            => _ids[x, y];

        public void SetBlockID(int x, int y, string id)
            => _ids[x, y] = id;

        public string[,] GetAllBlockID()
            => _ids;

        public Frame Copy()
        {
            string[,] copy = ArrayUtil.Copy(_ids);
            return new(copy);
        }

        public void Clear()
        {
            _ids = new string[0, 0];
        }

        public void Load(string[,] ids)
        {
            _ids = ids ?? throw new ArgumentNullException(nameof(ids));
        }

        public void Load(Frame frame)
        {
            if (frame is null)
                throw new ArgumentNullException(nameof(frame));

            _ids = frame._ids;
        }

        public BoundingBox Overwrite(Frame frame, Point location, Point offset)
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
            int endX = offsetX + frame.Width < Width ? offsetX + frame.Width - 1 : Width - 1;
            int endY = offsetY + frame.Height < Height ? offsetY + frame.Height - 1: Height - 1;
            for (int x1 = startX1, x2 = startX2; x1 <= endX; x1++, x2++)
                for (int y1 = startY1, y2 = startY2; y1 <= endY; y1++, y2++)
                {
                    string newBlcokID = frame.GetBlockID(x2, y2);
                    if (!string.IsNullOrEmpty(newBlcokID))
                        _ids[x1, y1] = newBlcokID;
                }

            return new(startY1, endY, startX1, endX);
        }

        public BoundingBox Overwrite(Frame frame, Point location)
        {
            return Overwrite(frame, location, new(0, 0));
        }

        public bool DrawRow(int row, int start, int length, string id)
        {
            if (row < 0 || row > Height - 1)
                return false;

            if (start >= Width)
                return false;
            else if (start < 0)
            {
                length += start;
                if (length <= 0)
                    return false;
                start = 0;
            }

            int end = start + length - 1;
            if (end >= Width)
                end = Width - 1;

            for (int x = start; x <= end; x++)
                _ids[x, row] = id;

            return true;
        }

        public bool DrawColumn(int column, int start, int length, string id)
        {
            if (column < 0 || column > Width - 1)
                return false;

            if (start >= Height)
                return false;
            else if (start < 0)
            {
                length += start;
                if (length <= 0)
                    return false;
                start = 0;
            }
            int end = start + length - 1;
            if (end >= Height)
                end = Height - 1;

            for (int y = start; y <= end; y++)
                _ids[column, y] = id;

            return true;
        }

        public bool FillRow(int row, string id)
        {
            if (row < 0 || row > Height - 1)
                return false;

            for (int x = 0; x < Width; x++)
                _ids[x, row] = id;

            return true;
        }

        public bool FillColumn(int column, string id)
        {
            if (column < 0 || column > Width - 1)
                return false;

            for (int y = 0; y < Height; y++)
                _ids[column, y] = id;

            return true;
        }

        public bool FillLeft(string id, int count)
        {
            if (count <= 0)
                return false;

            if (count > Width)
                count = Width;
            for (int x = 0; x < count; x++)
                for (int y = 0; y < Height; y++)
                    _ids[x, y] = id;

            return true;
        }

        public bool FillRight(string id, int count)
        {
            if (count <= 0)
                return false;

            if (count > Width)
                count = Width;
            for (int x = Width - 1; x >= Width - count; x--)
                for (int y = 0; y < Height; y++)
                    _ids[x, y] = id;

            return true;
        }

        public bool FillTop(string id, int count)
        {
            if (count <= 0)
                return false;

            if (count > Height)
                count = Height;
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < count; y++)
                    _ids[x, y] = id;

            return true;
        }

        public bool FillBottom(string id, int count)
        {
            if (count <= 0)
                return false;

            if (count > Height)
                count = Height;
            for (int x = 0; x < Width; x++)
                for (int y = Height - 1; y >= Height - count; y--)
                    _ids[x, y] = id;

            return true;
        }

        public bool FillBorder(string id, int count)
        {
            if (count <= 0)
                return false;

            return FillLeft(id, count) & FillRight(id, count) & FillTop(id, count) & FillBottom(id, count);
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

        public static List<ScreenPixel> GetDifferencesPixels(Frame frame1, Frame frame2)
        {
            if (frame1 is null)
                throw new ArgumentNullException(nameof(frame1));
            if (frame2 is null)
                throw new ArgumentNullException(nameof(frame2));
            if (frame1.Width != frame2.Width || frame1.Height != frame2.Height)
                throw new ArgumentException("相同尺寸的帧才能获取差异");

            List<ScreenPixel> result = new();
            int width = frame2.Width;
            int height = frame2.Height;
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    string newBlockID = frame2.GetBlockID(x, y);
                    if (newBlockID != frame1.GetBlockID(x, y))
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
