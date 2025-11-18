using QuanLib.Minecraft.API;
using QuanLib.Minecraft.API.Packet;
using QuanLib.Minecraft.Command.Senders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Instance.CommandSenders
{
    public class McapiCommandSender : ITwowayCommandSender, IOnewayCommandSender
    {
        public McapiCommandSender(McapiClient minecraftApiClient)
        {
            ArgumentNullException.ThrowIfNull(minecraftApiClient, nameof(minecraftApiClient));

            McapiClient = minecraftApiClient;
        }

        public McapiClient McapiClient { get; }

        public string SendCommand(string command)
        {
            ArgumentException.ThrowIfNullOrEmpty(command, nameof(command));

            return McapiClient.SendCommandAsync(command)
               .ConfigureAwait(false)
               .GetAwaiter()
               .GetResult();
        }

        public Task<string> SendCommandAsync(string command)
        {
            ArgumentException.ThrowIfNullOrEmpty(command, nameof(command));

            return McapiClient.SendCommandAsync(command);
        }

        public string[] SendBatchCommand(IList<string> commands)
        {
            ArgumentNullException.ThrowIfNull(commands, nameof(commands));

            return McapiClient.SendBatchCommandAsync(commands)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        public Task<string[]> SendBatchCommandAsync(IList<string> commands)
        {
            ArgumentNullException.ThrowIfNull(commands, nameof(commands));

            return McapiClient.SendBatchCommandAsync(commands);
        }

        public Task<string[]> SendDelayBatchCommandAsync(IList<string> commands, Task? delay)
        {
            ArgumentNullException.ThrowIfNull(commands, nameof(commands));

            return McapiClient.SendDelayBatchCommandAsync(commands, delay);
        }

        public void SendOnewayCommand(string command)
        {
            ArgumentException.ThrowIfNullOrEmpty(command, nameof(command));

            McapiClient.SendOnewayCommandAsync(command).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task SendOnewayCommandAsync(string command)
        {
            ArgumentException.ThrowIfNullOrEmpty(command, nameof(command));

            return McapiClient.SendOnewayCommandAsync(command);
        }

        public void SendOnewayBatchCommand(IList<string> commands)
        {
            ArgumentNullException.ThrowIfNull(commands, nameof(commands));

            McapiClient.SendOnewayBatchCommandAsync(commands).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task SendOnewayBatchCommandAsync(IList<string> commands)
        {
            ArgumentNullException.ThrowIfNull(commands, nameof(commands));

            return McapiClient.SendOnewayBatchCommandAsync(commands);
        }

        public Task SendOnewayDelayBatchCommandAsync(IList<string> commands, Task? delay)
        {
            ArgumentNullException.ThrowIfNull(commands, nameof(commands));

            return McapiClient.SendOnewayDelayBatchCommandAsync(commands, delay);
        }

        public void SendOnewayBatchSetBlock(IList<WorldBlock> arguments)
        {
            ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

            McapiClient.SendOnewayBatchSetBlockAsync(arguments).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task SendOnewayBatchSetBlockAsync(IList<WorldBlock> arguments)
        {
            ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

            return McapiClient.SendOnewayBatchSetBlockAsync(arguments);
        }

        public Task SendOnewayDelayBatchSetBlockAsync(IList<WorldBlock> arguments, Task? delay)
        {
            ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

            return McapiClient.SendOnewayDelayBatchSetBlockAsync(arguments, delay);
        }

        public TimeSpan Ping()
        {
            long start = Stopwatch.GetTimestamp();
            McapiClient.SendCommandAsync("time query gametime").ConfigureAwait(false).GetAwaiter().GetResult();
            return Stopwatch.GetElapsedTime(start);
        }

        public async Task<TimeSpan> PingAsync()
        {
            long start = Stopwatch.GetTimestamp();
            await McapiClient.SendCommandAsync("time query gametime").ConfigureAwait(false);
            return Stopwatch.GetElapsedTime(start);
        }
    }
}
