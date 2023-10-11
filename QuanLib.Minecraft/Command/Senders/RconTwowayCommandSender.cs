using CoreRCON;
using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Senders
{
    public class RconTwowayCommandSender : ITwowayCommandSender
    {
        public RconTwowayCommandSender(RCON rcon)
        {
            RCON = rcon ?? throw new ArgumentNullException(nameof(rcon));
            _synchronized = new();
        }

        private readonly Synchronized _synchronized;

        public RCON RCON { get; }

        public string SendCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
                throw new ArgumentException($"“{nameof(command)}”不能为 null 或空。", nameof(command));

            return _synchronized.Invoke(() => RCON.SendCommandAsync(command).Result);
        }

        public async Task<string> SendCommandAsync(string command)
        {
            if (string.IsNullOrEmpty(command))
                throw new ArgumentException($"“{nameof(command)}”不能为 null 或空。", nameof(command));

            return await _synchronized.Invoke(() => RCON.SendCommandAsync(command));
        }

        public string[] SendBatchCommand(IEnumerable<string> commands)
        {
            if (commands is null)
                throw new ArgumentNullException(nameof(commands));

            return _synchronized.Invoke(() =>
            {
                List<string> result = new();
                foreach (string command in commands)
                    result.Add(RCON.SendCommandAsync(command).Result);
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
                    result.Add(await RCON.SendCommandAsync(command));
                return result.ToArray();
            }
        }

        public void WaitForResponse()
        {
            RCON.SendCommandAsync("time query gametime").Wait();
        }

        public async Task WaitForResponseAsync()
        {
            await RCON.SendCommandAsync("time query gametime");
        }
    }
}
