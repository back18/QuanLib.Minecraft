using CoreRCON;
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
    public class RconTwowayCommandSender : ITwowayCommandSender
    {
        public RconTwowayCommandSender(RCON rcon)
        {
            ArgumentNullException.ThrowIfNull(rcon, nameof(rcon));

            RCON = rcon;
            _synchronized = new();
        }

        private readonly Synchronized _synchronized;

        public RCON RCON { get; }

        public string SendCommand(string command)
        {
            ArgumentException.ThrowIfNullOrEmpty(command, nameof(command));

            return _synchronized.Invoke(() => RCON.SendCommandAsync(command).ConfigureAwait(false).GetAwaiter().GetResult());
        }

        public Task<string> SendCommandAsync(string command)
        {
            ArgumentException.ThrowIfNullOrEmpty(command, nameof(command));

            return _synchronized.Invoke(() => RCON.SendCommandAsync(command));
        }

        public string[] SendBatchCommand(IList<string> commands)
        {
            ArgumentNullException.ThrowIfNull(commands, nameof(commands));

            return _synchronized.Invoke(() =>
            {
                List<string> result = new();
                foreach (string command in commands)
                    result.Add(RCON.SendCommandAsync(command).ConfigureAwait(false).GetAwaiter().GetResult());
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
                    result.Add(await RCON.SendCommandAsync(command).ConfigureAwait(false));
                return result.ToArray();
            }
        }

        public Task<string[]> SendDelayBatchCommandAsync(IList<string> commands, Task? delay)
        {
            return SendBatchCommandAsync(commands);
        }

        public TimeSpan Ping()
        {
            long start = Stopwatch.GetTimestamp();
            RCON.SendCommandAsync("time query gametime").ConfigureAwait(false).GetAwaiter().GetResult();
            return Stopwatch.GetElapsedTime(start);
        }

        public async Task<TimeSpan> PingAsync()
        {
            long start = Stopwatch.GetTimestamp();
            await RCON.SendCommandAsync("time query gametime").ConfigureAwait(false);
            return Stopwatch.GetElapsedTime(start);
        }
    }
}
