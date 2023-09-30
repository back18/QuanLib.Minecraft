using QuanLib.Core;
using QuanLib.Minecraft.API.Packet;
using QuanLib.Minecraft.Command.Senders;
using System;
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
            MinecraftApiClient = minecraftApiClient ?? throw new ArgumentNullException(nameof(minecraftApiClient));
            _semaphore = new(1);

            WaitForResponseCallback += OnWaitForResponseCallback;
        }

        private readonly SemaphoreSlim _semaphore;

        private Task? _task;

        public McapiClient MinecraftApiClient { get; }

        public event EventHandler<ICommandSender, EventArgs> WaitForResponseCallback;

        protected virtual void OnWaitForResponseCallback(ICommandSender sender, EventArgs e) { }

        public string SendCommand(string command)
        {
            _semaphore.Wait();
            try
            {
                _task?.Wait();
                _task = null;
                return MinecraftApiClient.SendCommandAsync(command).Result;
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
            await _semaphore.WaitAsync();
            try
            {
                _task?.Wait();
                Task<string> task = MinecraftApiClient.SendCommandAsync(command);
                _task = task;
                return await task;
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
            _semaphore.Wait();
            try
            {
                WaitForResponse();
                _task = null;
                return MinecraftApiClient.SendBatchCommandAsync(commands.ToArray()).Result;
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
            await _semaphore.WaitAsync();
            try
            {
                await WaitForResponseAsync();
                Task<string[]> task = MinecraftApiClient.SendBatchCommandAsync(commands.ToArray());
                _task = task;
                return await task;
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

        public void SendOnewayCommand(string command)
        {
            _semaphore.Wait();
            try
            {
                _task?.Wait();
                _task = null;
                MinecraftApiClient.SendOnewayCommandAsync(command).Wait();
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

        public async Task SendOnewayCommandAsync(string command)
        {
            await _semaphore.WaitAsync();
            try
            {
                _task?.Wait();
                _task = MinecraftApiClient.SendOnewayCommandAsync(command);
                await _task;
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

        public void SendOnewayBatchCommand(IEnumerable<string> commands)
        {
            _semaphore.Wait();
            try
            {
                WaitForResponse();
                _task = null;
                MinecraftApiClient.SendOnewayBatchCommandAsync(commands.ToArray()).Wait();
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

        public async Task SendOnewayBatchCommandAsync(IEnumerable<string> commands)
        {
            await _semaphore.WaitAsync();
            try
            {
                await WaitForResponseAsync();
                _task = MinecraftApiClient.SendOnewayBatchCommandAsync(commands.ToArray());
                await _task;
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

        public void SendOnewayBatchSetBlock(IEnumerable<ISetBlockArgument> arguments)
        {
            _semaphore.Wait();
            try
            {
                WaitForResponse();
                _task = null;
                MinecraftApiClient.SendBatchSetBlockAsync(arguments).Wait();
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

        public async Task SendOnewayBatchSetBlockAsync(IEnumerable<ISetBlockArgument> arguments)
        {
            await _semaphore.WaitAsync();
            try
            {
                await WaitForResponseAsync();
                _task = MinecraftApiClient.SendBatchSetBlockAsync(arguments);
                await _task;
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
            _task?.Wait();
            MinecraftApiClient.SendCommandAsync("time query gametime").Wait();
            WaitForResponseCallback.Invoke(this, EventArgs.Empty);
        }

        public async Task WaitForResponseAsync()
        {
            _task?.Wait();
            await MinecraftApiClient.SendCommandAsync("time query gametime");
            WaitForResponseCallback.Invoke(this, EventArgs.Empty);
        }
    }
}
