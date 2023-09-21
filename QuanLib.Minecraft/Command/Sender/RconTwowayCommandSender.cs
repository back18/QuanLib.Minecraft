using CoreRCON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Sender
{
    public class RconTwowayCommandSender : ITwowayCommandSender
    {
        public RconTwowayCommandSender(RCON rcon)
        {
            RCON = rcon ?? throw new ArgumentNullException(nameof(rcon));
            _semaphore = new(1);
        }

        private readonly SemaphoreSlim _semaphore;

        public RCON RCON { get; }

        public string SendCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
                throw new ArgumentException($"“{nameof(command)}”不能为 null 或空。", nameof(command));

            _semaphore.Wait();
            try
            {
                return RCON.SendCommandAsync(command).Result;
            }
            catch
            {
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<string> SendCommandAsync(string command)
        {
            if (string.IsNullOrEmpty(command))
                throw new ArgumentException($"“{nameof(command)}”不能为 null 或空。", nameof(command));

            await _semaphore.WaitAsync();
            try
            {
                return await RCON.SendCommandAsync(command);
            }
            catch
            {
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public string[] SendBatchCommand(IEnumerable<string> commands)
        {
            if (commands is null)
                throw new ArgumentNullException(nameof(commands));

            List<string> result = new();
            _semaphore.Wait();
            try
            {
                WaitForResponse();
                foreach (string command in commands)
                    result.Add(RCON.SendCommandAsync(command).Result);
                return result.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<string[]> SendBatchCommandAsync(IEnumerable<string> commands)
        {
            if (commands is null)
                throw new ArgumentNullException(nameof(commands));

            List<string> result = new();
            await _semaphore.WaitAsync();
            try
            {
                await WaitForResponseAsync();
                foreach (string command in commands)
                    result.Add(await RCON.SendCommandAsync(command));
                return result.ToArray();
            }
            catch
            {
                throw;
            }
            finally
            {
                _semaphore.Release();
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
