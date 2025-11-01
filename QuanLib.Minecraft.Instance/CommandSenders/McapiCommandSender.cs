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

            return McapiClient.SendCommandAsync(command).Result;
        }

        public async Task<string> SendCommandAsync(string command)
        {
            ArgumentException.ThrowIfNullOrEmpty(command, nameof(command));

            return await McapiClient.SendCommandAsync(command);
        }

        public string[] SendBatchCommand(IList<string> commands)
        {
            ArgumentNullException.ThrowIfNull(commands, nameof(commands));

            return McapiClient.SendBatchCommandAsync(commands).Result;
        }

        public async Task<string[]> SendBatchCommandAsync(IList<string> commands)
        {
            ArgumentNullException.ThrowIfNull(commands, nameof(commands));

            return await McapiClient.SendBatchCommandAsync(commands);
        }

        public async Task<string[]> SendDelayBatchCommandAsync(IList<string> commands, Task? delay)
        {
            ArgumentNullException.ThrowIfNull(commands, nameof(commands));

            return await McapiClient.SendDelayBatchCommandAsync(commands, delay);
        }

        public void SendOnewayCommand(string command)
        {
            ArgumentException.ThrowIfNullOrEmpty(command, nameof(command));

            McapiClient.SendOnewayCommandAsync(command).Wait();
        }

        public async Task SendOnewayCommandAsync(string command)
        {
            ArgumentException.ThrowIfNullOrEmpty(command, nameof(command));

            await McapiClient.SendOnewayCommandAsync(command);
        }

        public void SendOnewayBatchCommand(IList<string> commands)
        {
            ArgumentNullException.ThrowIfNull(commands, nameof(commands));

            McapiClient.SendOnewayBatchCommandAsync(commands).Wait();
        }

        public async Task SendOnewayBatchCommandAsync(IList<string> commands)
        {
            ArgumentNullException.ThrowIfNull(commands, nameof(commands));

            await McapiClient.SendOnewayBatchCommandAsync(commands);
        }

        public async Task SendOnewayDelayBatchCommandAsync(IList<string> commands, Task? delay)
        {
            ArgumentNullException.ThrowIfNull(commands, nameof(commands));

            await McapiClient.SendOnewayDelayBatchCommandAsync(commands, delay);
        }

        public void SendOnewayBatchSetBlock(IList<WorldBlock> arguments)
        {
            ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

            McapiClient.SendOnewayBatchSetBlockAsync(arguments).Wait();
        }

        public async Task SendOnewayBatchSetBlockAsync(IList<WorldBlock> arguments)
        {
            ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

            await McapiClient.SendOnewayBatchSetBlockAsync(arguments);
        }

        public async Task SendOnewayDelayBatchSetBlockAsync(IList<WorldBlock> arguments, Task? delay)
        {
            ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

            await McapiClient.SendOnewayDelayBatchSetBlockAsync(arguments, delay);
        }

        public TimeSpan Ping()
        {
            long start = Stopwatch.GetTimestamp();
            McapiClient.SendCommandAsync("time query gametime").Wait();
            return Stopwatch.GetElapsedTime(start);
        }

        public async Task<TimeSpan> PingAsync()
        {
            long start = Stopwatch.GetTimestamp();
            await McapiClient.SendCommandAsync("time query gametime");
            return Stopwatch.GetElapsedTime(start);
        }
    }
}
