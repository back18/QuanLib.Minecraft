using log4net.Core;
using QuanLib.Core;
using QuanLib.Core.Events;
using QuanLib.Minecraft.MinecraftLogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public class ServerConsole : RunnableBase
    {
        public ServerConsole(StreamReader reader, StreamWriter writer, Func<Type, LogImpl> logger) : base(logger)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));

            ReadLine += OnReadLine;
        }

        private readonly StreamReader _reader;

        private readonly StreamWriter _writer;

        private ConsoleTask? _task;

        public event EventHandler<ServerConsole, TextEventArgs> ReadLine;

        protected virtual void OnReadLine(ServerConsole sender, TextEventArgs e)
        {
            if (_task is not null)
            {
                MinecraftLog log = new(e.Text);
                _task.Complete(log.Message);
            }
        }

        protected override void Run()
        {
            while (IsRuning && !_reader.EndOfStream)
            {
                string? line = _reader.ReadLine();
                if (line is null)
                    continue;

                ReadLine.Invoke(this, new(line));
            }
        }

        public void WriteLine()
        {
            _writer.WriteLine();
        }

        public void WriteLine(char value)
        {
            _writer.WriteLine(value);
        }

        public void WriteLine(string? value)
        {
            _writer.WriteLine(value);
        }

        public void WriteLine<T>(T? value)
        {
            _writer.WriteLine(value?.ToString());
        }

        public async Task WriteLineAsync()
        {
            await _writer.WriteLineAsync();
        }

        public async Task WriteLineAsync(char value)
        {
            await _writer.WriteLineAsync(value);
        }

        public async Task WriteLineAsync(string? value)
        {
            await _writer.WriteLineAsync(value);
        }

        public async Task WriteLineAsync<T>(T? value)
        {
            await _writer.WriteLineAsync(value?.ToString());
        }

        public void Write(char value)
        {
            _writer.Write(value);
        }

        public void Write(string? value)
        {
            _writer.Write(value);
        }

        public void Write<T>(T? value)
        {
            _writer.Write(value?.ToString());
        }

        public async Task WriteAsync(char value)
        {
            await _writer.WriteAsync(value);
        }

        public async Task WriteAsync(string? value)
        {
            await _writer.WriteAsync(value);
        }

        public async Task WriteAsync<T>(T? value)
        {
            await _writer.WriteAsync(value?.ToString());
        }

        public async Task<string> SendCommandAsync(string command)
        {
            if (string.IsNullOrEmpty(command))
                throw new ArgumentException($"“{nameof(command)}”不能为 null 或空。", nameof(command));

            if (_task is not null)
                await _task.WaitForCompleteAsync();

            Task send = _writer.WriteLineAsync(command);
            _task = new(command, send);
            string output = await _task.WaitForCompleteAsync() ?? string.Empty;
            _task = null;
            return output;
        }

        private class ConsoleTask
        {
            public ConsoleTask(string input, Task send)
            {
                _semaphore = new(0);
                State = ConsoleTaskState.Sending;

                _input = input ?? throw new ArgumentNullException(nameof(input));
                _send = send ?? throw new ArgumentNullException(nameof(send));
                _receive = WaitForReceiveAsync();
            }

            private readonly SemaphoreSlim _semaphore;

            private readonly Task _send;

            private readonly Task _receive;

            private readonly string _input;

            private string? _output;

            public ConsoleTaskState State { get; private set; }

            public bool IsCompleted => State is ConsoleTaskState.Completed or ConsoleTaskState.Timeout;

            internal void Complete(string output)
            {
                if (output is null)
                    throw new ArgumentNullException(nameof(output));

                if (State != ConsoleTaskState.Sending && State != ConsoleTaskState.Receiving)
                    return;

                _output = output;
                _semaphore.Release();
                _receive.ContinueWith((task) =>
                {
                    State = ConsoleTaskState.Completed;
                });
            }

            public async Task<string?> WaitForCompleteAsync()
            {
                await _send;
                await _receive;
                return _output;
            }

            private async Task WaitForReceiveAsync()
            {
                await _send;
                State = ConsoleTaskState.Receiving;
                int millisecondsTimeout = 30 * 1000;
                Stopwatch stopwatch = Stopwatch.StartNew();
                await _semaphore.WaitAsync(millisecondsTimeout);
                stopwatch.Stop();
                if (stopwatch.ElapsedMilliseconds >= millisecondsTimeout)
                    State = ConsoleTaskState.Timeout;
            }
        }

        private enum ConsoleTaskState
        {
            Sending,

            Receiving,

            Timeout,

            Completed
        }
    }
}
