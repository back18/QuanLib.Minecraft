using CoreRCON;
using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Sender
{
    public class ConsoleCommandSender : ITwowayCommandSender, IOnewayCommandSender
    {
        public ConsoleCommandSender(ServerConsole serverConsole)
        {
            ServerConsole = serverConsole ?? throw new ArgumentNullException(nameof(serverConsole));
            _semaphore = new(1);

            WaitForResponseCallback += OnWaitForResponseCallback;
        }

        private readonly SemaphoreSlim _semaphore;

        private Task? _task;

        public ServerConsole ServerConsole { get; }

        public event EventHandler<ICommandSender, EventArgs> WaitForResponseCallback;

        protected virtual void OnWaitForResponseCallback(ICommandSender sender, EventArgs e) { }

        public string SendCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
                throw new ArgumentException($"“{nameof(command)}”不能为 null 或空。", nameof(command));

            _semaphore.Wait();
            try
            {
                _task?.Wait();
                _task = null;
                return ServerConsole.SendCommandAsync(command).Result;
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

            _semaphore.Wait();
            try
            {
                _task?.Wait();
                _task = null;
                return await ServerConsole.SendCommandAsync(command);
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
                    result.Add(ServerConsole.SendCommandAsync(command).Result);
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
                    result.Add(await ServerConsole.SendCommandAsync(command));
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

        public void SendOnewayCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
                throw new ArgumentException($"“{nameof(command)}”不能为 null 或空。", nameof(command));

            _semaphore.Wait();
            try
            {
                _task?.Wait();
                _task = null;
                ServerConsole.WriteLine(command);
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
            if (string.IsNullOrEmpty(command))
                throw new ArgumentException($"“{nameof(command)}”不能为 null 或空。", nameof(command));

            await _semaphore.WaitAsync();
            try
            {
                _task?.Wait();
                _task = ServerConsole.WriteLineAsync(command);
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
            if (commands is null)
                throw new ArgumentNullException(nameof(commands));

            string function = ToFunction(commands);
            _semaphore.Wait();
            try
            {
                WaitForResponse();
                _task = null;
                ServerConsole.WriteLine(function);
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
            if (commands is null)
                throw new ArgumentNullException(nameof(commands));

            string function = ToFunction(commands);
            await _semaphore.WaitAsync();
            try
            {
                await WaitForResponseAsync();
                _task = ServerConsole.WriteLineAsync(function);
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
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            string function = ToFunction(arguments);
            _semaphore.Wait();
            try
            {
                WaitForResponse();
                _task = null;
                ServerConsole.WriteLine(function);
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
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            string function = ToFunction(arguments);
            await _semaphore.WaitAsync();
            try
            {
                await WaitForResponseAsync();
                _task = ServerConsole.WriteLineAsync(function);
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
            ServerConsole.SendCommandAsync("time query gametime").Wait();
            WaitForResponseCallback.Invoke(this, EventArgs.Empty);
        }

        public async Task WaitForResponseAsync()
        {
            _task?.Wait();
            await ServerConsole.SendCommandAsync("time query gametime");
            WaitForResponseCallback.Invoke(this, EventArgs.Empty);
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
