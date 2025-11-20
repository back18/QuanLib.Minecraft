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

        public ServerConsole(Process process, ILoggerProvider? loggerProvider = null) : base(loggerProvider)
        {
            ArgumentNullException.ThrowIfNull(process, nameof(process));

            _process = process;
            _reader = StreamReader.Null;
            _writer = StreamWriter.Null;

            ReadLine += OnReadLine;
        }

        private readonly Process _process;

        private StreamReader _reader;

        private StreamWriter _writer;

        private ConsoleTask? _task;

        public bool ReadAvailable => _reader.Peek() > -1;

        public event ValueEventHandler<ServerConsole, ValueEventArgs<string>> ReadLine;

        protected virtual void OnReadLine(ServerConsole sender, ValueEventArgs<string> e)
        {
            string line = e.Argument;

            if (!line.StartsWith('['))
                return;

            if (_task is not null && _task.State == ConsoleTaskState.Receiving)
            {
                string message;
                int index = line.IndexOf(SEPARATOR);

                if (index == -1)
                    message = line;
                else
                    message = line[(index + SEPARATOR.Length)..];

                _task.Complete(message);
            }
        }

        protected override void Run()
        {
            int secondsTimeout = 0;
            while (true)
            {
                if (!IsRunning)
                    return;

                try
                {
                    DateTime startTime = _process.StartTime;    //成功获取到启动时间代表进程已启动，非则抛出 InvalidOperationException
                    break;
                }
                catch (InvalidOperationException invalidOperationException)
                {
                    Thread.Sleep(1000);
                    if (++secondsTimeout >= 5)
                        throw new TimeoutException($"在等待进程启动{secondsTimeout}秒后已超时", invalidOperationException);
                }
            }

            if (_process.HasExited)
                return;

            _reader = _process.StandardOutput;
            _writer = _process.StandardInput;

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

        public Task WriteLineAsync()
        {
            return _writer.WriteLineAsync();
        }

        public Task WriteLineAsync(char value)
        {
            return _writer.WriteLineAsync(value);
        }

        public Task WriteLineAsync(string? value)
        {
            return _writer.WriteLineAsync(value);
        }

        public Task WriteLineAsync(StringBuilder? value)
        {
            return _writer.WriteLineAsync(value);
        }

        public Task WriteLineAsync<T>(T? value)
        {
            return _writer.WriteLineAsync(value?.ToString());
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

        public Task WriteAsync(char value)
        {
            return _writer.WriteAsync(value);
        }

        public Task WriteAsync(string? value)
        {
            return _writer.WriteAsync(value);
        }

        public Task WriteAsync(StringBuilder? value)
        {
            return _writer.WriteAsync(value);
        }

        public Task WriteAsync<T>(T? value)
        {
            return _writer.WriteAsync(value?.ToString());
        }

        public Task FlushAsync()
        {
            return _writer.FlushAsync();
        }

        public async Task<string> SendCommandAsync(string command)
        {
            ArgumentException.ThrowIfNullOrEmpty(command, nameof(command));

            if (_task is not null)
                await _task.WaitForCompleteAsync().ConfigureAwait(false);

            _task = new(_writer, command);
            string output = await _task.Start().ConfigureAwait(false) ?? string.Empty;
            _task = null;
            return output;
        }

        private class ConsoleTask
        {
            public ConsoleTask(StreamWriter writer, string input)
            {
                ArgumentNullException.ThrowIfNull(writer, nameof(writer));
                ArgumentException.ThrowIfNullOrEmpty(input, nameof(input));

                State = ConsoleTaskState.Sending;

                _semaphore = new();
                _writer = writer;
                _input = input;
            }

            private readonly TaskSemaphore _semaphore;

            private readonly StreamWriter _writer;

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
            }

            public async Task<string?> Start()
            {
                await _writer.WriteLineAsync(_input).ConfigureAwait(false);

                State = ConsoleTaskState.Receiving;

                if (await _semaphore.WaitAsync(5000).ConfigureAwait(false))
                    State = ConsoleTaskState.Completed;
                else
                    State = ConsoleTaskState.Timeout;

                return _output;
            }

            public Task WaitForCompleteAsync()
            {
                return _semaphore.WaitAsync();
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
