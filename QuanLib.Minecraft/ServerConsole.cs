using QuanLib.Core;
using QuanLib.Core.Events;
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
        private const string SEPARATOR = ": ";

        public ServerConsole(StreamReader reader, StreamWriter writer, ILoggerGetter? loggerGetter = null) : base(loggerGetter)
        {
            ArgumentNullException.ThrowIfNull(reader, nameof(reader));
            ArgumentNullException.ThrowIfNull(writer, nameof(writer));

            _reader = reader;
            _writer = writer;

            ReadLine += OnReadLine;
        }

        private readonly StreamReader _reader;

        private readonly StreamWriter _writer;

        private ConsoleTask? _task;

        public event EventHandler<ServerConsole, EventArgs<string>> ReadLine;

        protected virtual void OnReadLine(ServerConsole sender, EventArgs<string> e)
        {
            if (_task is not null)
            {
                string message;
                int index = e.Argument.IndexOf(SEPARATOR);
                if (index == -1)
                    message = e.Argument;
                else
                    message = e.Argument[(index + SEPARATOR.Length)..];

                _task.Complete(message);
            }
        }

        protected override void Run()
        {
            while (IsRunning && !_reader.EndOfStream)
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
            ArgumentException.ThrowIfNullOrEmpty(command, nameof(command));

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
                ArgumentNullException.ThrowIfNull(input, nameof(input));
                ArgumentNullException.ThrowIfNull(send, nameof(send));

                _semaphore = new(0);
                State = ConsoleTaskState.Sending;

                _input = input;
                _send = send;
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
                ArgumentNullException.ThrowIfNull(output, nameof(output));

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
