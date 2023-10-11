using CoreRCON;
using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Senders
{
    public class ConsoleCommandSender : ITwowayCommandSender, IOnewayCommandSender
    {
        public ConsoleCommandSender(ServerConsole serverConsole)
        {
            ServerConsole = serverConsole ?? throw new ArgumentNullException(nameof(serverConsole));
            _synchronized = new();
        }

        private readonly Synchronized _synchronized;

        public ServerConsole ServerConsole { get; }

        public string SendCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
                throw new ArgumentException($"“{nameof(command)}”不能为 null 或空。", nameof(command));

            return _synchronized.Invoke(() => ServerConsole.SendCommandAsync(command).Result);
        }

        public async Task<string> SendCommandAsync(string command)
        {
            if (string.IsNullOrEmpty(command))
                throw new ArgumentException($"“{nameof(command)}”不能为 null 或空。", nameof(command));

            return await _synchronized.Invoke(() => ServerConsole.SendCommandAsync(command));
        }

        public string[] SendBatchCommand(IEnumerable<string> commands)
        {
            if (commands is null)
                throw new ArgumentNullException(nameof(commands));

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
            if (commands is null)
                throw new ArgumentNullException(nameof(commands));

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
            if (string.IsNullOrEmpty(command))
                throw new ArgumentException($"“{nameof(command)}”不能为 null 或空。", nameof(command));

            _synchronized.Invoke(() => ServerConsole.WriteLine(command));
        }

        public async Task SendOnewayCommandAsync(string command)
        {
            if (string.IsNullOrEmpty(command))
                throw new ArgumentException($"“{nameof(command)}”不能为 null 或空。", nameof(command));

            await _synchronized.Invoke(() => ServerConsole.WriteLineAsync(command));
        }

        public void SendOnewayBatchCommand(IEnumerable<string> commands)
        {
            if (commands is null)
                throw new ArgumentNullException(nameof(commands));

            string function = ToFunction(commands);
            _synchronized.Invoke(() => ServerConsole.WriteLine(function));
        }

        public async Task SendOnewayBatchCommandAsync(IEnumerable<string> commands)
        {
            if (commands is null)
                throw new ArgumentNullException(nameof(commands));

            string function = ToFunction(commands);
            await _synchronized.InvokeAsync(() => ServerConsole.WriteLineAsync(function));
        }

        public void SendOnewayBatchSetBlock(IEnumerable<ISetBlockArgument> arguments)
        {
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            string function = ToFunction(arguments);
            _synchronized.Invoke(() => ServerConsole.WriteLine(function));
        }

        public async Task SendOnewayBatchSetBlockAsync(IEnumerable<ISetBlockArgument> arguments)
        {
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            string function = ToFunction(arguments);
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

        private static string ToFunction(IEnumerable<ISetBlockArgument> arguments)
        {
            StringBuilder sb = new(arguments.Count() * 32);
            foreach (var argument in arguments)
            {
                sb.Append("setblock ");
                sb.Append(argument.Position.X);
                sb.Append(' ');
                sb.Append(argument.Position.Y);
                sb.Append(' ');
                sb.Append(argument.Position.Z);
                sb.Append(' ');
                sb.Append(argument.BlockID);
                sb.Append('\n');
            }
            sb.Length--;
            return sb.ToString();
        }
    }
}
