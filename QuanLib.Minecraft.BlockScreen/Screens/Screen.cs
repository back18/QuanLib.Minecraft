using QuanLib.ExceptionHelpe;
using QuanLib.Minecraft.BlockScreen.Frame;
using QuanLib.Minecraft.Vector;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Screens
{
    /// <summary>
    /// 屏幕
    /// </summary>
    public class Screen : IPlane, IScreenOptions
    {
        public Screen(IScreenOptions options) : this(options.StartPosition, options.Width, options.Height, options.XFacing, options.YFacing)
        {
        }

        public Screen(Vector3<int> startPosition, int width, int height, Facing xFacing, Facing yFacing)
        {
            ThrowHelper.TryThrowArgumentOutOfRangeException(-64, 319, startPosition.Y, "startPosition.Y");
            ThrowHelper.TryThrowArgumentOutOfMinException(1, width, nameof(width));
            ThrowHelper.TryThrowArgumentOutOfMinException(1, height, nameof(height));

            string xyFacing = xFacing.ToString() + yFacing.ToString();
            switch (xyFacing)
            {
                case "XpYm":
                case "YmXm":
                case "XmYp":
                case "YpXp":
                    Plane = Minecraft.Plane.XY;
                    NormalFacing = Facing.Zp;
                    PlaneCoordinate = startPosition.Z;
                    break;
                case "XmYm":
                case "YmXp":
                case "XpYp":
                case "YpXm":
                    Plane = Minecraft.Plane.XY;
                    NormalFacing = Facing.Zm;
                    PlaneCoordinate = startPosition.Z;
                    break;
                case "ZmYm":
                case "YmZp":
                case "ZpYp":
                case "YpZm":
                    Plane = Minecraft.Plane.ZY;
                    NormalFacing = Facing.Xp;
                    PlaneCoordinate = startPosition.X;
                    break;
                case "ZpYm":
                case "YmZm":
                case "ZmYp":
                case "YpZp":
                    Plane = Minecraft.Plane.ZY;
                    NormalFacing = Facing.Xm;
                    PlaneCoordinate = startPosition.X;
                    break;
                case "XpZp":
                case "ZpXm":
                case "XmZm":
                case "ZmXp":
                    Plane = Minecraft.Plane.XZ;
                    NormalFacing = Facing.Yp;
                    PlaneCoordinate = startPosition.Y;
                    break;
                case "XpZm":
                case "ZmXm":
                case "XmZp":
                case "ZpXp":
                    Plane = Minecraft.Plane.XZ;
                    NormalFacing = Facing.Ym;
                    PlaneCoordinate = startPosition.Y;
                    break;
                default:
                    throw new ArgumentException("xFacing 与 yFacing 不应该在同一轴向上");
            }

            int top, bottom, left, right;
            switch (yFacing)
            {
                case Facing.Xp:
                    top = startPosition.X;
                    bottom = startPosition.X + height - 1;
                    break;
                case Facing.Xm:
                    top = startPosition.X;
                    bottom = startPosition.X - height - 1;
                    break;
                case Facing.Yp:
                    top = startPosition.Y;
                    bottom = startPosition.Y + height - 1;
                    break;
                case Facing.Ym:
                    top = startPosition.Y;
                    bottom = startPosition.Y - height - 1;
                    break;
                case Facing.Zp:
                    top = startPosition.Z;
                    bottom = startPosition.Z + height - 1;
                    break;
                case Facing.Zm:
                    top = startPosition.Z;
                    bottom = startPosition.Z - height - 1;
                    break;
                default:
                    throw new InvalidOperationException();
            }
            switch (xFacing)
            {
                case Facing.Xp:
                    left = startPosition.X;
                    right = startPosition.X + height - 1;
                    break;
                case Facing.Xm:
                    left = startPosition.X;
                    right = startPosition.X - height - 1;
                    break;
                case Facing.Yp:
                    left = startPosition.Y;
                    right = startPosition.Y + height - 1;
                    break;
                case Facing.Ym:
                    left = startPosition.Y;
                    right = startPosition.Y - height - 1;
                    break;
                case Facing.Zp:
                    left = startPosition.Z;
                    right = startPosition.Z + height - 1;
                    break;
                case Facing.Zm:
                    left = startPosition.Z;
                    right = startPosition.Z - height - 1;
                    break;
                default:
                    throw new InvalidOperationException();
            }
            ScreenRange = new(top, bottom, left, right);

            StartPosition = startPosition;
            XFacing = xFacing;
            YFacing = yFacing;
            Width = width;
            Height = height;
            DefaultBackgroundBlcokID = "minecraft:smooth_stone";

            _chunks = new();

            ThrowHelper.TryThrowArgumentOutOfRangeException(-64, 319, EndPosition.Y, "EndPosition.Y");
        }

        private readonly List<SurfacePos> _chunks;

        public Vector3<int> StartPosition { get; }

        public Vector3<int> EndPosition => ToWorldPosition(new(Width - 1, Height - 1));

        public Vector3<int> CenterPosition => ToWorldPosition(ScreenCenterPosition);

        public Point ScreenCenterPosition => new(Width / 2, Height / 2);

        public Minecraft.Plane Plane { get; }

        public Facing NormalFacing { get; }

        public int PlaneCoordinate { get; }

        public Facing XFacing { get; }

        public Facing YFacing { get; }

        public int Width { get; }

        public int Height { get; }

        public Size Size => new(Width, Height);

        public int TotalPixels => Width * Height;

        public RectangleRange ScreenRange { get; }

        public string DefaultBackgroundBlcokID { get; set; }

        public ScreenInputHandler InputHandler
        {
            get
            {
                _InputHandler ??= new(this);
                return _InputHandler;
            }
        }
        private ScreenInputHandler? _InputHandler;

        public ScreenOutputHandler OutputHandler
        {
            get
            {
                _OutputHandler ??= new(this);
                return _OutputHandler;
            }
        }
        private ScreenOutputHandler? _OutputHandler;

        public void Fill()
        {
            OutputHandler.HandleOutput(ArrayFrame.BuildFrame(Width, Height, DefaultBackgroundBlcokID));
        }

        public void Clear()
        {
            OutputHandler.HandleOutput(ArrayFrame.BuildFrame(Width, Height, "minecraft:air"));
        }

        public void Start()
        {
            LoadScreenChunks();
            Fill();
        }

        public void Stop()
        {
            UnloadScreenChunks();
            Clear();
        }

        public void LoadScreenChunks()
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    var blockPos = ToWorldPosition(new(x, y));
                    SurfacePos chunkPos = MinecraftUtil.BlockPos2ChunkPos(new(blockPos.X, blockPos.Z));
                    if (!_chunks.Contains(chunkPos))
                        _chunks.Add(chunkPos);
                }

            foreach (var chunk in _chunks)
                MCOS.Instance.MinecraftServer.CommandHelper.AddForceLoadChunk(MinecraftUtil.ChunkPos2BlockPos(chunk));
        }

        public void UnloadScreenChunks()
        {
            foreach (var chunk in _chunks)
                MCOS.Instance.MinecraftServer.CommandHelper.RemoveForceLoadChunk(MinecraftUtil.ChunkPos2BlockPos(chunk));
        }

        public WorldPixel ToWorldPixel(ScreenPixel pixel)
        {
            return new(ToWorldPosition(pixel.Position), pixel.BlockID);
        }

        public Vector3<int> ToWorldPosition(Point pixel)
        {
            int? x = null;
            int? y = null;
            int? z = null;

            switch (XFacing)
            {
                case Facing.Xp:
                    x = StartPosition.X + pixel.X;
                    break;
                case Facing.Xm:
                    x = StartPosition.X - pixel.X;
                    break;
                case Facing.Yp:
                    y = StartPosition.Y + pixel.X;
                    break;
                case Facing.Ym:
                    y = StartPosition.Y - pixel.X;
                    break;
                case Facing.Zp:
                    z = StartPosition.Z + pixel.X;
                    break;
                case Facing.Zm:
                    z = StartPosition.Z - pixel.X;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            switch (YFacing)
            {
                case Facing.Xp:
                    x = StartPosition.X + pixel.Y;
                    break;
                case Facing.Xm:
                    x = StartPosition.X - pixel.Y;
                    break;
                case Facing.Yp:
                    y = StartPosition.Y + pixel.Y;
                    break;
                case Facing.Ym:
                    y = StartPosition.Y - pixel.Y;
                    break;
                case Facing.Zp:
                    z = StartPosition.Z + pixel.Y;
                    break;
                case Facing.Zm:
                    z = StartPosition.Z - pixel.Y;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            x ??= StartPosition.X;
            y ??= StartPosition.Y;
            z ??= StartPosition.Z;

            return new(x.Value, y.Value, z.Value);
        }

        public Point ToScreenPosition(Vector3<int> blockPos)
        {
            var x = XFacing switch
            {
                Facing.Xp => blockPos.X - StartPosition.X,
                Facing.Xm => StartPosition.X - blockPos.X,
                Facing.Yp => blockPos.Y - StartPosition.Y,
                Facing.Ym => StartPosition.Y - blockPos.Y,
                Facing.Zp => blockPos.Z - StartPosition.Z,
                Facing.Zm => StartPosition.Z - blockPos.Z,
                _ => throw new InvalidOperationException()
            };
            var y = YFacing switch
            {
                Facing.Xp => blockPos.X - StartPosition.X,
                Facing.Xm => StartPosition.X - blockPos.X,
                Facing.Yp => blockPos.Y - StartPosition.Y,
                Facing.Ym => StartPosition.Y - blockPos.Y,
                Facing.Zp => blockPos.Z - StartPosition.Z,
                Facing.Zm => StartPosition.Z - blockPos.Z,
                _ => throw new InvalidOperationException()
            };

            return new(x, y);
        }

        public bool IncludedOnScreen(Point pixel)
        {
            return pixel.X >= 0 && pixel.Y >= 0 && pixel.X < Width && pixel.Y < Height;
        }

        public bool IncludedOnScreen(Vector3<int> blockPos)
        {
            bool isScreenPlane = NormalFacing switch
            {
                Facing.Xp or Facing.Xm => blockPos.X == PlaneCoordinate,
                Facing.Yp or Facing.Ym => blockPos.Y == PlaneCoordinate,
                Facing.Zp or Facing.Zm => blockPos.Z == PlaneCoordinate,
                _ => throw new InvalidOperationException()
            };
            return isScreenPlane && IncludedOnScreen(ToScreenPosition(blockPos));
        }

        public override string ToString()
        {
            return $"StartPosition={StartPosition}, Width={Width}, Height={Height}, XFacing={XFacing}, YFacing={YFacing}";
        }

        public static Screen CreateScreen(Vector3<int> startPosition, Vector3<int> endPosition, Facing normalFacing)
        {
            ThrowHelper.TryThrowArgumentOutOfRangeException(-64, 319, startPosition.Y, "startPosition.Y");
            ThrowHelper.TryThrowArgumentOutOfRangeException(-64, 319, endPosition.Y, "endPosition.Y");

            Facing xFacing, yFacing;
            int width, height;
            if (startPosition.X == endPosition.X && (normalFacing == Facing.Xp || normalFacing == Facing.Xm))
            {
                bool swap = false;
                switch (normalFacing)
                {
                    case Facing.Xp:
                        if (startPosition.Z > endPosition.Z && startPosition.Y > endPosition.Y)
                        {
                            xFacing = Facing.Zm;
                            yFacing = Facing.Ym;
                        }
                        else if (startPosition.Z <= endPosition.Z && startPosition.Y <= endPosition.Y)
                        {
                            xFacing = Facing.Zp;
                            yFacing = Facing.Yp;
                        }
                        else if (startPosition.Z > endPosition.Z && startPosition.Y <= endPosition.Y)
                        {
                            swap = true;
                            xFacing = Facing.Yp;
                            yFacing = Facing.Zm;
                        }
                        else if (endPosition.Z <= endPosition.Z && startPosition.Y > endPosition.Y)
                        {
                            swap = true;
                            xFacing = Facing.Ym;
                            yFacing = Facing.Zp;
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                        break;
                    case Facing.Xm:
                        if (startPosition.Z > endPosition.Z && startPosition.Y > endPosition.Y)
                        {
                            swap = true;
                            xFacing = Facing.Ym;
                            yFacing = Facing.Zm;
                        }
                        else if (startPosition.Z <= endPosition.Z && startPosition.Y <= endPosition.Y)
                        {
                            swap = true;
                            xFacing = Facing.Yp;
                            yFacing = Facing.Zp;
                        }
                        else if (startPosition.Z > endPosition.Z && startPosition.Y <= endPosition.Y)
                        {
                            xFacing = Facing.Zm;
                            yFacing = Facing.Yp;
                        }
                        else if (endPosition.Z <= endPosition.Z && startPosition.Y > endPosition.Y)
                        {
                            xFacing = Facing.Zp;
                            yFacing = Facing.Ym;
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                if (swap)
                {
                    width = Math.Abs(startPosition.Y - endPosition.Y) + 1;
                    height = Math.Abs(startPosition.Z - endPosition.Z) + 1;
                }
                else
                {
                    width = Math.Abs(startPosition.Z - endPosition.Z) + 1;
                    height = Math.Abs(startPosition.Y - endPosition.Y) + 1;
                }
            }
            else if (startPosition.Y == endPosition.Y && (normalFacing == Facing.Yp || normalFacing == Facing.Ym))
            {
                bool swap = false;
                switch (normalFacing)
                {
                    case Facing.Yp:
                        if (startPosition.X > endPosition.X && startPosition.Z > endPosition.Z)
                        {
                            xFacing = Facing.Xm;
                            yFacing = Facing.Zm;
                        }
                        else if (startPosition.X <= endPosition.X && startPosition.Z <= endPosition.Z)
                        {
                            xFacing = Facing.Xp;
                            yFacing = Facing.Zp;
                        }
                        else if (startPosition.X > endPosition.X && startPosition.Z <= endPosition.Z)
                        {
                            swap = true;
                            xFacing = Facing.Zp;
                            yFacing = Facing.Xm;
                        }
                        else if (startPosition.X <= endPosition.X && startPosition.Z > endPosition.Z)
                        {
                            swap = true;
                            xFacing = Facing.Zm;
                            yFacing = Facing.Xp;
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                        break;
                    case Facing.Ym:
                        if (startPosition.X > endPosition.X && startPosition.Z > endPosition.Z)
                        {
                            swap = true;
                            xFacing = Facing.Zm;
                            yFacing = Facing.Xm;
                        }
                        else if (startPosition.X <= endPosition.X && startPosition.Z <= endPosition.Z)
                        {
                            swap = true;
                            xFacing = Facing.Zp;
                            yFacing = Facing.Xp;
                        }
                        else if (startPosition.X > endPosition.X && startPosition.Z <= endPosition.Z)
                        {
                            xFacing = Facing.Xm;
                            yFacing = Facing.Zp;
                        }
                        else if (startPosition.X <= endPosition.X && startPosition.Z > endPosition.Z)
                        {
                            xFacing = Facing.Xp;
                            yFacing = Facing.Zm;
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                if (swap)
                {
                    width = Math.Abs(startPosition.Z - endPosition.Z) + 1;
                    height = Math.Abs(startPosition.X - endPosition.X) + 1;
                }
                else
                {
                    width = Math.Abs(startPosition.X - endPosition.X) + 1;
                    height = Math.Abs(startPosition.Z - endPosition.Z) + 1;
                }
            }
            else if (startPosition.Z == endPosition.Z && (normalFacing == Facing.Zp || normalFacing == Facing.Zm))
            {
                bool swap = false;
                switch (normalFacing)
                {
                    case Facing.Zp:
                        if (startPosition.X > endPosition.X && startPosition.Y > endPosition.Y)
                        {
                            swap = true;
                            xFacing = Facing.Ym;
                            yFacing = Facing.Xm;
                        }
                        else if (startPosition.X <= endPosition.X && startPosition.Y <= endPosition.Y)
                        {
                            swap = true;
                            xFacing = Facing.Yp;
                            yFacing = Facing.Xp;
                        }
                        else if (startPosition.X > endPosition.X && startPosition.Y <= endPosition.Y)
                        {
                            xFacing = Facing.Xm;
                            yFacing = Facing.Yp;
                        }
                        else if (startPosition.X <= endPosition.X && startPosition.Y > endPosition.Y)
                        {
                            xFacing = Facing.Xp;
                            yFacing = Facing.Ym;
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                        break;
                    case Facing.Zm:
                        if (startPosition.X > endPosition.X && startPosition.Y > endPosition.Y)
                        {
                            xFacing = Facing.Xm;
                            yFacing = Facing.Ym;
                        }
                        else if (startPosition.X <= endPosition.X && startPosition.Y <= endPosition.Y)
                        {
                            xFacing = Facing.Xp;
                            yFacing = Facing.Yp;
                        }
                        else if (startPosition.X > endPosition.X && startPosition.Y <= endPosition.Y)
                        {
                            swap = true;
                            xFacing = Facing.Yp;
                            yFacing = Facing.Xm;
                        }
                        else if (startPosition.X <= endPosition.X && startPosition.Y > endPosition.Y)
                        {
                            swap = true;
                            xFacing = Facing.Ym;
                            yFacing = Facing.Xp;
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                if (swap)
                {
                    width = Math.Abs(startPosition.Y - endPosition.Y) + 1;
                    height = Math.Abs(startPosition.X - endPosition.X) + 1;
                }
                else
                {
                    width = Math.Abs(startPosition.X - endPosition.X) + 1;
                    height = Math.Abs(startPosition.Y - endPosition.Y) + 1;
                }
            }
            else
            {
                throw new ArgumentException("屏幕的起始点与截止点不在一个平面");
            }

            return new(startPosition, width, height, xFacing, yFacing);
        }

        public static void Replace(Screen? oldScreen, Screen newScreen)
        {
            if (newScreen is null)
                throw new ArgumentNullException(nameof(newScreen));

            if (oldScreen is null || oldScreen.OutputHandler.LastFrame is null)
            {
                newScreen.Fill();
                return;
            }

            if (oldScreen.Plane != newScreen.Plane || oldScreen.PlaneCoordinate != newScreen.PlaneCoordinate)
            {
                oldScreen.Clear();
                newScreen.Fill();
                return;
            }

            ArrayFrame? frame = null;
            if (oldScreen.DefaultBackgroundBlcokID == newScreen.DefaultBackgroundBlcokID &&
                oldScreen.StartPosition == oldScreen.StartPosition &&
                oldScreen.XFacing == newScreen.XFacing &&
                oldScreen.YFacing == newScreen.YFacing)
            {
                if (newScreen.OutputHandler.LastFrame is null)
                    newScreen.OutputHandler.LastFrame = ArrayFrame.BuildFrame(newScreen.Width, newScreen.Height, newScreen.DefaultBackgroundBlcokID);

                if (newScreen.Width > oldScreen.Width)
                {
                    for (int x = oldScreen.Width; x < newScreen.Width; x++)
                        for (int y = 0; y < newScreen.Height; y++)
                            newScreen.OutputHandler.LastFrame.SetBlockID(x, y, "minecraft:air");
                }
                else if (newScreen.Width < oldScreen.Width)
                {
                    frame ??= oldScreen.OutputHandler.LastFrame.Clone();
                    for (int x = newScreen.Width; x < oldScreen.Width; x++)
                        for (int y = 0; y < oldScreen.Height; y++)
                            frame.SetBlockID(x, y, "minecraft:air");
                }

                if (newScreen.Height > oldScreen.Height)
                {
                    for (int x = 0; x < newScreen.Width; x++)
                        for (int y = oldScreen.Height; y < newScreen.Height; y++)
                            newScreen.OutputHandler.LastFrame.SetBlockID(x, y, "minecraft:air");
                }
                else if (newScreen.Height < oldScreen.Height)
                {
                    frame ??= oldScreen.OutputHandler.LastFrame.Clone();
                    for (int x = 0; x < oldScreen.Width; x++)
                        for (int y = newScreen.Height; y < oldScreen.Height; y++)
                            frame.SetBlockID(x, y, "minecraft:air");
                }

                newScreen.Fill();
                if (frame is not null)
                    oldScreen.OutputHandler.HandleOutput(frame);
            }
            else
            {
                frame = oldScreen.OutputHandler.LastFrame.Clone();
                if (newScreen.OutputHandler.LastFrame is null)
                    newScreen.Fill();

                List<Vector3<int>> worlds = new();
                for (int x = 0; x < newScreen.Width; x++)
                    for (int y = 0; y < newScreen.Height; y++)
                        worlds.Add(newScreen.ToWorldPosition(new(x, y)));

                for (int x = 0; x < oldScreen.Width; x++)
                    for (int y = 0; y < oldScreen.Height; y++)
                    {
                        if (!worlds.Contains(oldScreen.ToWorldPosition(new(x, y))))
                            frame.SetBlockID(x, y, "minecraft:air");
                    }

                oldScreen.OutputHandler.HandleOutput(frame);
            }
        }
    }
}
