using QuanLib.Minecraft.BlockScreen.Frame;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen.Screens
{
    /// <summary>
    /// 屏幕输出处理
    /// </summary>
    public class ScreenOutputHandler
    {
        public ScreenOutputHandler(Screen owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        private readonly Screen _owner;

        private Task? previous;

        private Task? _current;

        public ArrayFrame? LastFrame { get; private set; }

        public bool IsGenerated => LastFrame is not null;

        public void WaitPrevious()
        {
            previous?.Wait();
        }

        public async Task WaitPreviousAsync()
        {
            if (previous is not null)
                await previous;
        }

        public void HandleOutput(ArrayFrame frame)
        {
            List<ScreenPixel> pixels = GetDifferencesPixels(frame);
            LastFrame = frame;
            if (pixels.Count > 0)
            {
                if (MCOS.GetMCOS().EnableAccelerationEngine)
                {
                    AccelerationEngineSend(MCOS.GetMCOS().AccelerationEngine, pixels);
                }
                else
                {
                    ICommandSender sender = MCOS.GetMCOS().MinecraftServer.CommandSender;
                    if (sender is IStandardInputCommandSender standardInput)
                    {
                        StandardInputCommandSend(standardInput, pixels);
                    }
                    else if (sender is IBytesCommandSender bytes)
                    {
                        BytesCommandSend(bytes, pixels);
                    }
                    else
                    {
                        CommandSend(sender, pixels);
                    }
                }
            }
        }

        public async Task HandleOutputAsync(ArrayFrame frame)
        {
            previous = _current;
            _current = PrivateShowNewFrameAsync(frame);
            await _current;
        }

        private async Task PrivateShowNewFrameAsync(ArrayFrame frame)
        {
            List<ScreenPixel> pixels = GetDifferencesPixels(frame);
            LastFrame = frame;
            if (pixels.Count > 0)
            {
                if (MCOS.GetMCOS().EnableAccelerationEngine)
                {
                    await AccelerationEngineSendAsync(MCOS.GetMCOS().AccelerationEngine, pixels);
                }
                else
                {
                    ICommandSender sender = MCOS.GetMCOS().MinecraftServer.CommandSender;
                    if (sender is IStandardInputCommandSender standardInput)
                    {
                        await StandardInputCommandSendAsync(standardInput, pixels);
                    }
                    else if (sender is IBytesCommandSender bytes)
                    {
                        await BytesCommandSendAsync(bytes, pixels);
                    }
                    else
                    {
                        await CommandSendAsync(sender, pixels);
                    }
                }
            }
        }

        private void AccelerationEngineSend(AccelerationEngine ae, List<ScreenPixel> pixels)
        {
            byte[] bytes = ToSetBlockDataPacket(pixels);
            HandleWaitAndCallbacks();
            ae.SendData(bytes);
        }

        private async Task AccelerationEngineSendAsync(AccelerationEngine ae, List<ScreenPixel> pixels)
        {
            byte[] bytes = ToSetBlockDataPacket(pixels);
            HandleWaitAndCallbacks();
            await ae.SendDataAsync(bytes);
        }

        private void StandardInputCommandSend(IStandardInputCommandSender sender, List<ScreenPixel> pixels)
        {
            string function = ToSetBlockFunction(pixels);
            HandleWaitAndCallbacks();
            sender.SendCommand(function);
            MCOS.GetMCOS().MinecraftServer.CommandHelper.SendCommand("time query gametime");
        }

        private async Task StandardInputCommandSendAsync(IStandardInputCommandSender sender, List<ScreenPixel> pixels)
        {
            string function = ToSetBlockFunction(pixels);
            HandleWaitAndCallbacks();
            await sender.SendCommandAsync(function);
            await MCOS.GetMCOS().MinecraftServer.CommandHelper.SendCommandAsync("time query gametime");
        }

        private void BytesCommandSend(IBytesCommandSender sender, List<ScreenPixel> pixels)
        {
            ConcurrentBag<byte[]> commands = ToSetBlockBytesList(sender, pixels);
            HandleWaitAndCallbacks();
            sender.SendAllCommand(commands);
        }

        private async Task BytesCommandSendAsync(IBytesCommandSender sender, List<ScreenPixel> pixels)
        {
            ConcurrentBag<byte[]> commands = ToSetBlockBytesList(sender, pixels);
            HandleWaitAndCallbacks();
            await sender.SendAllCommandAsync(commands);
        }

        private void CommandSend(ICommandSender sender, List<ScreenPixel> pixels)
        {
            List<string> commands = ToSetblockCommands(pixels);
            HandleWaitAndCallbacks();
            sender.SendAllCommand(commands);
        }

        private async Task CommandSendAsync(ICommandSender sender, List<ScreenPixel> pixels)
        {
            List<string> commands = ToSetblockCommands(pixels);
            HandleWaitAndCallbacks();
            await sender.SendAllCommandAsync(commands);
        }

        private byte[] ToSetBlockDataPacket(List<ScreenPixel> pixels)
        {
            List<WorldPixel> worldPixels = new(pixels.Count);
            foreach (var pixel in pixels)
                worldPixels.Add(_owner.ToWorldPixel(pixel));
            byte[] bytes = AccelerationEngine.DataPacket.ToDataPacket(worldPixels).ToBytes();
            return bytes;
        }

        private string ToSetBlockFunction(List<ScreenPixel> pixels)
        {
            StringBuilder sb = new(pixels.Count * 17);
            foreach (ScreenPixel pixel in pixels)
            {
                WorldPixel worldPixel = _owner.ToWorldPixel(pixel);
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

        private ConcurrentBag<byte[]> ToSetBlockBytesList(IBytesCommandSender sender, List<ScreenPixel> pixels)
        {
            ConcurrentBag<byte[]> commands = new();
            Parallel.ForEach(pixels, (pixel) =>
            {
                commands.Add(sender.CommandToBytes(_owner.ToWorldPixel(pixel).ToSetBlock()));
            });
            while (commands.Count < pixels.Count)
                Thread.Yield();
            return commands;
        }

        private List<string> ToSetblockCommands(List<ScreenPixel> pixels)
        {
            List<string> commands = new(pixels.Count);
            foreach (ScreenPixel pixel in pixels)
                commands.Add(_owner.ToWorldPixel(pixel).ToSetBlock());
            return commands;
        }

        private void HandleWaitAndCallbacks()
        {
            if (previous is not null)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                MCOS.GetMCOS().ScreenManager.WaitAllScreenPrevious();
                stopwatch.Stop();
                if (stopwatch.ElapsedMilliseconds > 50)
                {
                    MCOS.GetMCOS()._callbacks.Clear();
                }
                else
                {
                    while (MCOS.GetMCOS()._callbacks.TryDequeue(out var callback))
                        callback.Invoke();
                }
            }
        }

        private List<ScreenPixel> GetDifferencesPixels(ArrayFrame frame)
        {
            if (frame is null)
                throw new ArgumentNullException(nameof(frame));
            if (frame.Width != _owner.Width || frame.Height != _owner.Height)
                throw new ArgumentException("帧尺寸不一致");

            List<ScreenPixel> pixels;
            if (LastFrame is null)
                pixels = frame.GetAllPixel();
            else
                pixels = ArrayFrame.GetDifferencesPixels(LastFrame, frame);

            return pixels;
        }
    }
}
