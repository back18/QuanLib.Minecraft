using QuanLib.Core;
using QuanLib.Minecraft.API.Packet;
using QuanLib.Minecraft.CommandSenders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API
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

        public string[] SendBatchCommand(IEnumerable<string> commands)
        {
            ArgumentNullException.ThrowIfNull(commands, nameof(commands));

            return McapiClient.SendBatchCommandAsync(commands.ToArray()).Result;
        }

        public async Task<string[]> SendBatchCommandAsync(IEnumerable<string> commands)
        {
            ArgumentNullException.ThrowIfNull(commands, nameof(commands));

            return await McapiClient.SendBatchCommandAsync(commands.ToArray());
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

        public void SendOnewayBatchCommand(IEnumerable<string> commands)
        {
            ArgumentNullException.ThrowIfNull(commands, nameof(commands));

            McapiClient.SendOnewayBatchCommandAsync(commands.ToArray()).Wait();
        }

        public async Task SendOnewayBatchCommandAsync(IEnumerable<string> commands)
        {
            ArgumentNullException.ThrowIfNull(commands, nameof(commands));

            await McapiClient.SendOnewayBatchCommandAsync(commands.ToArray());
        }

        public void SendOnewayBatchSetBlock(IEnumerable<WorldBlock> blocks)
        {
            ArgumentNullException.ThrowIfNull(blocks, nameof(blocks));

            McapiClient.SendOnewayBatchSetBlockAsync(blocks).Wait();
        }

        public async Task SendOnewayBatchSetBlockAsync(IEnumerable<WorldBlock> blocks)
        {
            ArgumentNullException.ThrowIfNull(blocks, nameof(blocks));

            await McapiClient.SendOnewayBatchSetBlockAsync(blocks);
        }

        public void WaitForResponse()
        {
            McapiClient.SendCommandAsync("time query gametime").Wait();
        }

        public async Task WaitForResponseAsync()
        {
            await McapiClient.SendCommandAsync("time query gametime");
        }
    }
}
