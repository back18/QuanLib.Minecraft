using CoreRCON;
using QuanLib.Core;
using QuanLib.Minecraft.Command.Senders;
using System;
using System.Collections.Generic;
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

            return _synchronized.Invoke(() => ServerConsole.SendCommandAsync(command).Result);
        }

        public async Task<string> SendCommandAsync(string command)
        {
            ArgumentException.ThrowIfNullOrEmpty(command, nameof(command));

            return await _synchronized.Invoke(() => ServerConsole.SendCommandAsync(command));
        }

        public string[] SendBatchCommand(IEnumerable<string> commands)
        {
            ArgumentNullException.ThrowIfNull(commands, nameof(commands));

            return _synchronized.Invoke(() =>
            {
                List<string> result = new();
                foreach (string command in commands)
                    result.Add(ServerConsole.SendCommandAsync(command).Result);
                return result.ToArray();
            });
        }

        public async Task<string[]> SendBatchCommandAsync(IEnumerable<string> commands)
        {
            ArgumentNullException.ThrowIfNull(commands, nameof(commands));

            return await _synchronized.InvokeAsync(() => FuncAsync(commands));

            async Task<string[]> FuncAsync(IEnumerable<string> commands)
            {
                List<string> result = new();
                foreach (string command in commands)
                    result.Add(await ServerConsole.SendCommandAsync(command));
                return result.ToArray();
            }
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

        public void SendOnewayBatchCommand(IEnumerable<string> commands)
        {
            ArgumentNullException.ThrowIfNull(commands, nameof(commands));

            string function = ToFunction(commands);
            _synchronized.Invoke(() => ServerConsole.WriteLine(function));
        }

        public async Task SendOnewayBatchCommandAsync(IEnumerable<string> commands)
        {
            ArgumentNullException.ThrowIfNull(commands, nameof(commands));

            string function = ToFunction(commands);
            await _synchronized.InvokeAsync(() => ServerConsole.WriteLineAsync(function));
        }

        public void SendOnewayBatchSetBlock(IEnumerable<WorldBlock> blocks)
        {
            ArgumentNullException.ThrowIfNull(blocks, nameof(blocks));

            string function = ToFunction(blocks);
            _synchronized.Invoke(() => ServerConsole.WriteLine(function));
        }

        public async Task SendOnewayBatchSetBlockAsync(IEnumerable<WorldBlock> blocks)
        {
            ArgumentNullException.ThrowIfNull(blocks, nameof(blocks));

            string function = ToFunction(blocks);
            await _synchronized.InvokeAsync(() => ServerConsole.WriteLineAsync(function));
        }

        public void WaitForResponse()
        {
            ServerConsole.SendCommandAsync("time query gametime").Wait();
        }

        public async Task WaitForResponseAsync()
        {
            await ServerConsole.SendCommandAsync("time query gametime");
        }

        private static string ToFunction(IEnumerable<string> commands)
        {
            StringBuilder sb = new(commands.Count() * 32);
            foreach (var command in commands)
            {
                sb.Append(command);
                sb.Append('\n');
            }
            sb.Length--;
            return sb.ToString();
        }

        private static string ToFunction(IEnumerable<WorldBlock> blocks)
        {
            StringBuilder sb = new(blocks.Count() * 32);
            foreach (var block in blocks)
                sb.AppendFormat("setblock {0} {1} {2} {3}\n", block.Position.X, block.Position.Y, block.Position.Z, block.BlockID);
            sb.Length--;
            return sb.ToString();
        }
    }
}
