using QuanLib.Core;
using QuanLib.Minecraft.Command.Senders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Instance.CommandSenders
{
    public class ConsoleCommandSender : ITwowayCommandSender, IOnewayCommandSender
    {
        public ConsoleCommandSender(ServerConsole serverConsole)
        {
            ArgumentNullException.ThrowIfNull(serverConsole, nameof(serverConsole));

            ServerConsole = serverConsole;
            _synchronized = new();
        }

        private readonly Synchronized _synchronized;

        public ServerConsole ServerConsole { get; }

        public string SendCommand(string command)
        {
            ArgumentException.ThrowIfNullOrEmpty(command, nameof(command));

            return _synchronized.Invoke(() => ServerConsole.SendCommandAsync(command).ConfigureAwait(false).GetAwaiter().GetResult());
        }

        public async Task<string> SendCommandAsync(string command)
        {
            ArgumentException.ThrowIfNullOrEmpty(command, nameof(command));

            return await _synchronized.Invoke(() => ServerConsole.SendCommandAsync(command));
        }

        public string[] SendBatchCommand(IList<string> commands)
        {
            ArgumentNullException.ThrowIfNull(commands, nameof(commands));

            return _synchronized.Invoke(() =>
            {
                List<string> result = new();
                foreach (string command in commands)
                    result.Add(ServerConsole.SendCommandAsync(command).ConfigureAwait(false).GetAwaiter().GetResult());
                return result.ToArray();
            });
        }

        public async Task<string[]> SendBatchCommandAsync(IList<string> commands)
        {
            ArgumentNullException.ThrowIfNull(commands, nameof(commands));

            return await _synchronized.InvokeAsync(() => FuncAsync(commands));

            async Task<string[]> FuncAsync(IList<string> commands)
            {
                List<string> result = new();
                foreach (string command in commands)
                    result.Add(await ServerConsole.SendCommandAsync(command));
                return result.ToArray();
            }
        }

        public async Task<string[]> SendDelayBatchCommandAsync(IList<string> commands, Task? delay)
        {
            return await SendBatchCommandAsync(commands).ConfigureAwait(false);
        }

        public void SendOnewayCommand(string command)
        {
            ArgumentException.ThrowIfNullOrEmpty(command, nameof(command));

            _synchronized.Invoke(() => ServerConsole.WriteLine(command));
        }

        public async Task SendOnewayCommandAsync(string command)
        {
            ArgumentException.ThrowIfNullOrEmpty(command, nameof(command));

            await _synchronized.Invoke(() => ServerConsole.WriteLineAsync(command));
        }

        public void SendOnewayBatchCommand(IList<string> commands)
        {
            ArgumentNullException.ThrowIfNull(commands, nameof(commands));

            string function = ToFunction(commands);
            _synchronized.Invoke(() => ServerConsole.WriteLine(function));
        }

        public async Task SendOnewayBatchCommandAsync(IList<string> commands)
        {
            ArgumentNullException.ThrowIfNull(commands, nameof(commands));

            string function = await Task.Run(() => ToFunction(commands)).ConfigureAwait(false);
            await _synchronized.InvokeAsync(() => ServerConsole.WriteLineAsync(function));
        }

        public async Task SendOnewayDelayBatchCommandAsync(IList<string> commands, Task? delay)
        {
            ArgumentNullException.ThrowIfNull(commands, nameof(commands));

            string function = await Task.Run(() => ToFunction(commands)).ConfigureAwait(false);

            if (delay is not null)
                await delay.ConfigureAwait(false);

            await _synchronized.InvokeAsync(() => ServerConsole.WriteLineAsync(function));
        }

        public void SendOnewayBatchSetBlock(IList<WorldBlock> arguments)
        {
            ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

            string function = ToFunction(arguments);
            _synchronized.Invoke(() => ServerConsole.WriteLine(function));
        }

        public async Task SendOnewayBatchSetBlockAsync(IList<WorldBlock> arguments)
        {
            ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

            string function = await Task.Run(() => ToFunction(arguments)).ConfigureAwait(false);
            await _synchronized.InvokeAsync(() => ServerConsole.WriteLineAsync(function));
        }

        public async Task SendOnewayDelayBatchSetBlockAsync(IList<WorldBlock> arguments, Task? delay)
        {
            ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

            string function = await Task.Run(() => ToFunction(arguments)).ConfigureAwait(false);

            if (delay is not null)
                await delay.ConfigureAwait(false);

            await _synchronized.InvokeAsync(() => ServerConsole.WriteLineAsync(function));
        }

        public TimeSpan Ping()
        {
            long start = Stopwatch.GetTimestamp();
            ServerConsole.SendCommandAsync("time query gametime").ConfigureAwait(false).GetAwaiter().GetResult();
            return Stopwatch.GetElapsedTime(start);
        }

        public async Task<TimeSpan> PingAsync()
        {
            long start = Stopwatch.GetTimestamp();
            await ServerConsole.SendCommandAsync("time query gametime").ConfigureAwait(false);
            return Stopwatch.GetElapsedTime(start);
        }

        private static string ToFunction(IList<string> commands)
        {
            if (commands.Count == 0)
                return string.Empty;

            StringBuilder stringBuilder = new(commands.Count * 32);
            foreach (var command in commands)
                stringBuilder.AppendLine(command);

            if (stringBuilder.Length <= Environment.NewLine.Length)
                return string.Empty;

            stringBuilder.Length -= Environment.NewLine.Length;
            return stringBuilder.ToString();
        }

        private static string ToFunction(IList<WorldBlock> blocks)
        {
            if (blocks.Count == 0)
                return string.Empty;

            StringBuilder stringBuilder = new(blocks.Count * 32);
            foreach (var block in blocks)
                stringBuilder.AppendLine($"setblock {block.Position.X} {block.Position.Y} {block.Position.Z} {block.BlockId}");

            stringBuilder.Length -= Environment.NewLine.Length;
            return stringBuilder.ToString();
        }
    }
}
