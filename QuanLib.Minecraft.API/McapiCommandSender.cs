using QuanLib.Core;
using QuanLib.Minecraft.API.Packet;
using QuanLib.Minecraft.Command.Senders;
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
            McapiClient = minecraftApiClient ?? throw new ArgumentNullException(nameof(minecraftApiClient));
        }

        public McapiClient McapiClient { get; }

        public string SendCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
                throw new ArgumentException($"“{nameof(command)}”不能为 null 或空。", nameof(command));

            return McapiClient.SendCommandAsync(command).Result;
        }

        public async Task<string> SendCommandAsync(string command)
        {
            if (string.IsNullOrEmpty(command))
                throw new ArgumentException($"“{nameof(command)}”不能为 null 或空。", nameof(command));

            return await McapiClient.SendCommandAsync(command);
        }

        public string[] SendBatchCommand(IEnumerable<string> commands)
        {
            if (commands is null)
                throw new ArgumentNullException(nameof(commands));

            return McapiClient.SendBatchCommandAsync(commands.ToArray()).Result;
        }

        public async Task<string[]> SendBatchCommandAsync(IEnumerable<string> commands)
        {
            if (commands is null)
                throw new ArgumentNullException(nameof(commands));

            return await McapiClient.SendBatchCommandAsync(commands.ToArray());
        }

        public void SendOnewayCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
                throw new ArgumentException($"“{nameof(command)}”不能为 null 或空。", nameof(command));

            McapiClient.SendOnewayCommandAsync(command).Wait();
        }

        public async Task SendOnewayCommandAsync(string command)
        {
            if (string.IsNullOrEmpty(command))
                throw new ArgumentException($"“{nameof(command)}”不能为 null 或空。", nameof(command));

            await McapiClient.SendOnewayCommandAsync(command);
        }

        public void SendOnewayBatchCommand(IEnumerable<string> commands)
        {
            if (commands is null)
                throw new ArgumentNullException(nameof(commands));

            McapiClient.SendOnewayBatchCommandAsync(commands.ToArray()).Wait();
        }

        public async Task SendOnewayBatchCommandAsync(IEnumerable<string> commands)
        {
            if (commands is null)
                throw new ArgumentNullException(nameof(commands));

            await McapiClient.SendOnewayBatchCommandAsync(commands.ToArray());
        }

        public void SendOnewayBatchSetBlock(IEnumerable<ISetBlockArgument> arguments)
        {
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            McapiClient.SendOnewayBatchSetBlockAsync(arguments).Wait();
        }

        public async Task SendOnewayBatchSetBlockAsync(IEnumerable<ISetBlockArgument> arguments)
        {
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            await McapiClient.SendOnewayBatchSetBlockAsync(arguments);
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
