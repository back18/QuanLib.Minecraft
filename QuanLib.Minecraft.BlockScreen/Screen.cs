﻿using QuanLib.ExceptionHelpe;
using QuanLib.Minecraft.BlockScreen.Frame;
using QuanLib.Minecraft.Vectors;
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

namespace QuanLib.Minecraft.BlockScreen
{
    /// <summary>
    /// 屏幕
    /// </summary>
    public class Screen : IMCOSComponent
    {
        public Screen(Vector3<int> startPosition, Facing xFacing, Facing yFacing, int width, int height)
        {
            ThrowHelper.TryThrowArgumentOutOfRangeException(-64, 319, startPosition.Y, "startPosition.Y");
            ThrowHelper.TryThrowArgumentOutOfMinException(1, width, nameof(width));
            ThrowHelper.TryThrowArgumentOutOfMinException(1, height, nameof(height));

            string xyFacing = xFacing.ToString() + yFacing.ToString();
            switch (xyFacing)
            {
                case "ZmYm":
                case "YmZp":
                case "ZpYp":
                case "YpZm":
                    NormalFacing = Facing.Xp;
                    PlaneCoordinate = startPosition.X;
                    break;
                case "ZpYm":
                case "YmZm":
                case "ZmYp":
                case "YpZp":
                    NormalFacing = Facing.Xm;
                    PlaneCoordinate = startPosition.X;
                    break;
                case "XpYm":
                case "YmXm":
                case "XmYp":
                case "YpXp":
                    NormalFacing = Facing.Zp;
                    PlaneCoordinate = startPosition.Z;
                    break;
                case "XmYm":
                case "YmXp":
                case "XpYp":
                case "Ypym":
                    NormalFacing = Facing.Zm;
                    PlaneCoordinate = startPosition.Z;
                    break;
                case "XpZp":
                case "ZpXm":
                case "XmZm":
                case "ZmXp":
                    NormalFacing = Facing.Yp;
                    PlaneCoordinate = startPosition.Y;
                    break;
                case "XpZm":
                case "ZmXm":
                case "XmZp":
                case "ZpXp":
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

        public MCOS MCOS
        {
            get
            {
                if (_MCOS is null)
                    throw new InvalidOperationException();
                return _MCOS;
            }
            internal set => _MCOS = value;
        }
        private MCOS? _MCOS;

        public Vector3<int> WorldStartPosition { get; }

        public Vector3<int> WorldCenterPosition => ToWorldPosition(ScreenCenterPosition);

        public Vector3<int> WorldEndPosition => ToWorldPosition(new(Width - 1, Height - 1));

        public Point ScreenCenterPosition => new(Width / 2, Height / 2);

        public int PlaneCoordinate { get; }

        public Facing XFacing { get; }

        public Facing YFacing { get; }

        public Facing NormalFacing { get; }

        public int Width { get; }

        public int Height { get; }

        public int TotalPixels => Width * Height;

        public RectangleRange ScreenRange { get; }

        public string DefaultBackgroundBlcokID { get; set; }

        public ArrayFrame? LastFrame { get; private set; }

        public static Screen CreateScreen(Vector3<int> startPosition, Vector3<int> endPosition)
        {
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

        public void ShowNewFrame(ArrayFrame frame, Task? wait = null)
        {
            List<ScreenPixel> pixels = GetDifferencesPixels(frame);
            LastFrame = frame;
            if (pixels.Count > 0)
            {
                if (MCOS.EnableAccelerationEngine)
                {
                    AccelerationEngineSend(MCOS.AccelerationEngine, pixels, wait);
                }
                else
                {
                    ICommandSender sender = MCOS.MinecraftServer.CommandSender;
                    if (sender is IStandardInputCommandSender standardInput)
                    {
                        StandardInputCommandSend(standardInput, pixels, wait);
                    }
                    else if (sender is IBytesCommandSender bytes)
                    {
                        BytesCommandSend(bytes, pixels, wait);
                    }
                    else
                    {
                        CommandSend(sender, pixels, wait);
                    }
                }
            }
        }

        public async Task ShowNewFrameAsync(ArrayFrame frame, Task? wait = null)
        {
            List<ScreenPixel> pixels = GetDifferencesPixels(frame);
            LastFrame = frame;
            if (pixels.Count > 0)
            {
                if (MCOS.EnableAccelerationEngine)
                {
                    await AccelerationEngineSendAsync(MCOS.AccelerationEngine, pixels, wait);
                }
                else
                {
                    ICommandSender sender = MCOS.MinecraftServer.CommandSender;
                    if (sender is IStandardInputCommandSender standardInput)
                    {
                        await StandardInputCommandSendAsync(standardInput, pixels, wait);
                    }
                    else if (sender is IBytesCommandSender bytes)
                    {
                        await BytesCommandSendAsync(bytes, pixels, wait);
                    }
                    else
                    {
                        await CommandSendAsync(sender, pixels, wait);
                    }
                }
            }
        }

        private void AccelerationEngineSend(AccelerationEngine ae, List<ScreenPixel> pixels, Task? wait)
        {
            List<WorldPixel> worldPixels = new(pixels.Count);
            foreach (var pixel in pixels)
                worldPixels.Add(ToWorldPixel(pixel));
            byte[] bytes = AccelerationEngine.DataPacket.ToDataPacket(worldPixels).ToBytes();

            HandleWaitAndCallbacks(wait);
            ae.SendData(bytes);
        }

        private async Task AccelerationEngineSendAsync(AccelerationEngine ae, List<ScreenPixel> pixels, Task? wait)
        {
            List<WorldPixel> worldPixels = new(pixels.Count);
            foreach (var pixel in pixels)
                worldPixels.Add(ToWorldPixel(pixel));
            byte[] bytes = AccelerationEngine.DataPacket.ToDataPacket(worldPixels).ToBytes();

            HandleWaitAndCallbacks(wait);
            await ae.SendDataAsync(bytes);
        }

        private void CommandSend(ICommandSender sender, List<ScreenPixel> pixels, Task? wait)
        {
            List<string> commands = new(pixels.Count);
            foreach (ScreenPixel pixel in pixels)
                commands.Add(ToWorldPixel(pixel).ToSetBlock());

            HandleWaitAndCallbacks(wait);
            sender.SendAllCommand(commands);
        }

        private async Task CommandSendAsync(ICommandSender sender, List<ScreenPixel> pixels, Task? wait)
        {
            List<string> commands = new(pixels.Count);
            foreach (ScreenPixel pixel in pixels)
                commands.Add(ToWorldPixel(pixel).ToSetBlock());

            HandleWaitAndCallbacks(wait);
            await sender.SendAllCommandAsync(commands);
        }

        private void StandardInputCommandSend(IStandardInputCommandSender sender, List<ScreenPixel> pixels, Task? wait)
        {
            string function = ToSetBlockFunction(pixels);
            HandleWaitAndCallbacks(wait);
            sender.SendCommand(function);
            MCOS.MinecraftServer.CommandHelper.SendCommand("time query gametime");
        }

        private async Task StandardInputCommandSendAsync(IStandardInputCommandSender sender, List<ScreenPixel> pixels, Task? wait)
        {
            string function = ToSetBlockFunction(pixels);
            HandleWaitAndCallbacks(wait);
            await sender.SendCommandAsync(function);
            await MCOS.MinecraftServer.CommandHelper.SendCommandAsync("time query gametime");
        }

        private void BytesCommandSend(IBytesCommandSender sender, List<ScreenPixel> pixels, Task? wait)
        {
            ConcurrentBag<byte[]> commands = ToSetBlockBytesList(sender, pixels);
            HandleWaitAndCallbacks(wait);
            sender.SendAllCommand(commands);
        }

        private async Task BytesCommandSendAsync(IBytesCommandSender sender, List<ScreenPixel> pixels, Task? wait)
        {
            ConcurrentBag<byte[]> commands = ToSetBlockBytesList(sender, pixels);
            HandleWaitAndCallbacks(wait);
            await sender.SendAllCommandAsync(commands);
        }

        private void HandleWaitAndCallbacks(Task? wait)
        {
            if (wait is not null)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                wait.Wait();
                stopwatch.Stop();
                if (stopwatch.ElapsedMilliseconds > 50)
                {
                    MCOS._callbacks.Clear();
                }
                else
                {
                    while (MCOS._callbacks.TryDequeue(out var callback))
                        callback.Invoke();
                }
            }
        }

        private List<ScreenPixel> GetDifferencesPixels(ArrayFrame frame)
        {
            if (frame is null)
                throw new ArgumentNullException(nameof(frame));
            if (frame.Width != Width || frame.Height != Height)
                throw new ArgumentException("参数必须为与屏幕相同尺寸的帧");

            List<ScreenPixel> pixels;
            if (LastFrame is null)
                pixels = frame.GetAllPixel();
            else
                pixels = ArrayFrame.GetDifferencesPixels(LastFrame, frame);

            return pixels;
        }

        public void Replace(Screen screen)
        {
            if (screen is null)
                throw new ArgumentNullException(nameof(screen));

            if (screen.LastFrame is null)
            {
                Fill();
                return;
            }

            ArrayFrame frame = ArrayFrame.BuildFrame(Width, Height, DefaultBackgroundBlcokID);

            List<WorldPixel> worldPixels1 = new();
            foreach (var pixel in screen.LastFrame.GetAllPixel())
                worldPixels1.Add(screen.ToWorldPixel(pixel));

            List<WorldPixel> worldPixels2 = new();
            foreach (var pixel in frame.GetAllPixel())
                worldPixels2.Add(ToWorldPixel(pixel));

            var differences = worldPixels1.Except(worldPixels2);

            if (differences.Any())
            {
                if (MCOS.EnableAccelerationEngine)
                {
                    byte[] bytes = AccelerationEngine.DataPacket.ToDataPacket(differences).ToBytes();
                    MCOS.AccelerationEngine.SendData(bytes);
                }
                else
                {

                }
            }
        }

        public void Fill()
        {
            ShowNewFrame(ArrayFrame.BuildFrame(Width, Height, DefaultBackgroundBlcokID));
        }

        public void Clear()
        {
            ShowNewFrame(ArrayFrame.BuildFrame(Width, Height, "minecraft:air"));
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
                MCOS.MinecraftServer.CommandHelper.AddForceLoadChunk(MinecraftUtil.ChunkPos2BlockPos(chunk));
        }

        public void UnloadScreenChunks()
        {
            foreach (var chunk in _chunks)
                MCOS.MinecraftServer.CommandHelper.RemoveForceLoadChunk(MinecraftUtil.ChunkPos2BlockPos(chunk));
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

        private ConcurrentBag<byte[]> ToSetBlockBytesList(IBytesCommandSender sender, List<ScreenPixel> pixels)
        {
            ConcurrentBag<byte[]> commands = new();
            Parallel.ForEach(pixels, (pixel) =>
            {
                commands.Add(sender.CommandToBytes(ToWorldPixel(pixel).ToSetBlock()));
            });
            while (commands.Count < pixels.Count)
                Thread.Yield();
            return commands;
        }

        private string ToSetBlockFunction(IEnumerable<ScreenPixel> pixels)
        {
            StringBuilder sb = new(pixels.Count() * 17);
            foreach (ScreenPixel pixel in pixels)
            {
                WorldPixel worldPixel = ToWorldPixel(pixel);
                sb.Append("setblock ");
                sb.Append(worldPixel.Position.X);
                sb.Append(' ');
                sb.Append(worldPixel.Position.Y);
                sb.Append(' ');
                sb.Append(worldPixel.Position.Z);
                sb.Append(' ');
                sb.Append(worldPixel.BlockID);
                sb.Append('\n');
            }
            sb.Length--;
            return sb.ToString();
        }
    }
}
