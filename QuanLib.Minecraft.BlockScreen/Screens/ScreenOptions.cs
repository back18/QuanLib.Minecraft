using QuanLib.Minecraft.Vector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Screens
{
    public readonly struct ScreenOptions : IScreenOptions
    {
        public ScreenOptions(IScreenOptions options)
        {
            StartPosition = options.StartPosition;
            Width = options.Width;
            Height = options.Height;
            XFacing = options.XFacing;
            YFacing = options.YFacing;
        }

        public ScreenOptions(Json json)
        {
            StartPosition = new(json.StartPosition[0], json.StartPosition[1], json.StartPosition[2]);
            Width = json.Width;
            Height = json.Height;
            XFacing = json.XFacing;
            YFacing = json.YFacing;
        }

        public Vector3<int> StartPosition { get; }

        public int Width { get; }

        public int Height { get; }

        public Facing XFacing { get; }

        public Facing YFacing { get; }

        public Json ToJson()
        {
            return new Json
            {
                StartPosition = new int[] { StartPosition.X, StartPosition.Y, StartPosition.Z },
                Width = Width,
                Height = Height,
                XFacing = XFacing,
                YFacing = YFacing,
            };
        }

        public override string ToString()
        {
            return $"StartPosition={StartPosition}, Width={Width}, Height={Height}, XFacing={XFacing}, YFacing={YFacing}";
        }

        public class Json
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
            public int[] StartPosition;

            public int Width;

            public int Height;

            public Facing XFacing;

            public Facing YFacing;
        }
    }
}
