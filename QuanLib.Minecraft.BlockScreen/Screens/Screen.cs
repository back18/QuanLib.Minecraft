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
    public class Screen : IPlaneSize
    {
        public Screen(Vector3<int> startPosition, Facing xFacing, Facing yFacing, int width, int height)
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
                    Plane = Plane.XY;
                    NormalFacing = Facing.Zp;
                    PlaneCoordinate = startPosition.Z;
                    break;
                case "XmYm":
                case "YmXp":
                case "XpYp":
                case "Ypym":
                    Plane = Plane.XY;
                    NormalFacing = Facing.Zm;
                    PlaneCoordinate = startPosition.Z;
                    break;
                case "ZmYm":
                case "YmZp":
                case "ZpYp":
                case "YpZm":
                    Plane = Plane.ZY;
                    NormalFacing = Facing.Xp;
                    PlaneCoordinate = startPosition.X;
                    break;
                case "ZpYm":
                case "YmZm":
                case "ZmYp":
                case "YpZp":
                    Plane = Plane.ZY;
                    NormalFacing = Facing.Xm;
                    PlaneCoordinate = startPosition.X;
                    break;
                case "XpZp":
                case "ZpXm":
                case "XmZm":
                case "ZmXp":
                    Plane = Plane.XZ;
                    NormalFacing = Facing.Yp;
                    PlaneCoordinate = startPosition.Y;
                    break;
                case "XpZm":
                case "ZmXm":
                case "XmZp":
                case "ZpXp":
                    Plane = Plane.XZ;
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

            WorldStartPosition = startPosition;
            XFacing = xFacing;
            YFacing = yFacing;
            Width = width;
            Height = height;
            DefaultBackgroundBlcokID = "minecraft:smooth_stone";

            _chunks = new();
        }

        private readonly List<SurfacePos> _chunks;

        public Vector3<int> WorldStartPosition { get; }

        public Vector3<int> WorldEndPosition => ToWorldPosition(new(Width - 1, Height - 1));

        public Vector3<int> WorldCenterPosition => ToWorldPosition(ScreenCenterPosition);

        public Point ScreenCenterPosition => new(Width / 2, Height / 2);

        public Plane Plane { get; }

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
                    x = WorldStartPosition.X + pixel.X;
                    break;
                case Facing.Xm:
                    x = WorldStartPosition.X - pixel.X;
                    break;
                case Facing.Yp:
                    y = WorldStartPosition.Y + pixel.X;
                    break;
                case Facing.Ym:
                    y = WorldStartPosition.Y - pixel.X;
                    break;
                case Facing.Zp:
                    z = WorldStartPosition.Z + pixel.X;
                    break;
                case Facing.Zm:
                    z = WorldStartPosition.Z - pixel.X;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            switch (YFacing)
            {
                case Facing.Xp:
                    x = WorldStartPosition.X + pixel.Y;
                    break;
                case Facing.Xm:
                    x = WorldStartPosition.X - pixel.Y;
                    break;
                case Facing.Yp:
                    y = WorldStartPosition.Y + pixel.Y;
                    break;
                case Facing.Ym:
                    y = WorldStartPosition.Y - pixel.Y;
                    break;
                case Facing.Zp:
                    z = WorldStartPosition.Z + pixel.Y;
                    break;
                case Facing.Zm:
                    z = WorldStartPosition.Z - pixel.Y;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            x ??= WorldStartPosition.X;
            y ??= WorldStartPosition.Y;
            z ??= WorldStartPosition.Z;

            return new(x.Value, y.Value, z.Value);
        }

        public Point ToScreenPosition(Vector3<int> blockPos)
        {
            var x = XFacing switch
            {
                Facing.Xp => blockPos.X - WorldStartPosition.X,
                Facing.Xm => WorldStartPosition.X - blockPos.X,
                Facing.Yp => blockPos.Y - WorldStartPosition.Y,
                Facing.Ym => WorldStartPosition.Y - blockPos.Y,
                Facing.Zp => blockPos.Z - WorldStartPosition.Z,
                Facing.Zm => WorldStartPosition.Z - blockPos.Z,
                _ => throw new InvalidOperationException()
            };
            var y = YFacing switch
            {
                Facing.Xp => blockPos.X - WorldStartPosition.X,
                Facing.Xm => WorldStartPosition.X - blockPos.X,
                Facing.Yp => blockPos.Y - WorldStartPosition.Y,
                Facing.Ym => WorldStartPosition.Y - blockPos.Y,
                Facing.Zp => blockPos.Z - WorldStartPosition.Z,
                Facing.Zm => WorldStartPosition.Z - blockPos.Z,
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

        public static Screen CreateScreen(Vector3<int> startPosition, Vector3<int> endPosition)
        {
            ThrowHelper.TryThrowArgumentOutOfRangeException(-64, 319, startPosition.Y, "startPosition.Y");
            ThrowHelper.TryThrowArgumentOutOfRangeException(-64, 319, endPosition.Y, "endPosition.Y");

            Facing xFacing, yFacing;
            int width, height;
            if (startPosition.X == endPosition.X)
            {
                if (startPosition.Z > endPosition.Z)
                    xFacing = Facing.Zm;
                else
                    xFacing = Facing.Zp;
                if (startPosition.Y > endPosition.Y)
                    yFacing = Facing.Ym;
                else
                    yFacing = Facing.Yp;

                width = Math.Abs(startPosition.Z - endPosition.Z) + 1;
                height = Math.Abs(startPosition.Y - endPosition.Y) + 1;
            }
            else if (startPosition.Y == endPosition.Y)
            {
                if (startPosition.X > endPosition.X)
                    xFacing = Facing.Xm;
                else
                    xFacing = Facing.Xp;
                if (startPosition.Z > endPosition.Z)
                    yFacing = Facing.Zm;
                else
                    yFacing = Facing.Zp;

                width = Math.Abs(startPosition.X - endPosition.X) + 1;
                height = Math.Abs(startPosition.Z - endPosition.Z) + 1;
            }
            else if (startPosition.Z == endPosition.Z)
            {
                if (startPosition.X > endPosition.X)
                    xFacing = Facing.Xm;
                else
                    xFacing = Facing.Xp;
                if (startPosition.Y > endPosition.Y)
                    yFacing = Facing.Ym;
                else
                    yFacing = Facing.Yp;

                width = Math.Abs(startPosition.X - endPosition.X) + 1;
                height = Math.Abs(startPosition.Y - endPosition.Y) + 1;
            }
            else
            {
                throw new ArgumentException("屏幕的起始点与截止点不在一个平面");
            }

            return new(startPosition, xFacing, yFacing, width, height);
        }

        public static void Replace(Screen? oldScreen, Screen newScreen)
        {
            if (newScreen is null)
                throw new ArgumentNullException(nameof(newScreen));

            if (oldScreen is null)
            {
                newScreen.Fill();
                return;
            }

            if (newScreen.OutputHandler.LastFrame is null)
            {
                newScreen.Fill();
            }

            if (oldScreen.Plane != newScreen.Plane || oldScreen.PlaneCoordinate != newScreen.PlaneCoordinate)
            {
                oldScreen.Clear();
            }
            else if (oldScreen.OutputHandler.LastFrame is not null)
            {
                if (newScreen.OutputHandler.LastFrame is null)
                    throw new InvalidOperationException();

                List<Vector3<int>> world = new();
                for (int x = 0; x < newScreen.Width; x++)
                    for (int y = 0; y < newScreen.Height; y++)
                        world.Add(newScreen.ToWorldPosition(new(x, y)));

                ArrayFrame frame = oldScreen.OutputHandler.LastFrame.Clone();
                for (int x = 0; x < oldScreen.Width; x++)
                    for (int y = 0; y < oldScreen.Height; y++)
                    {
                        if (!world.Contains(oldScreen.ToWorldPosition(new(x, y))))
                            frame.SetBlockID(x, y, "minecraft:air");
                    }

                oldScreen.OutputHandler.HandleOutput(frame);
            }
        }
    }
}
