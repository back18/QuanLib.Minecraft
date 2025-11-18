using QuanLib.Core;
using QuanLib.Minecraft.Command;
using QuanLib.Minecraft.Command.Senders;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Instance.CommandSenders
{
    public class RconOnewayCommandSender : UnmanagedRunnable, IOnewayCommandSender
    {
        public RconOnewayCommandSender(IPAddress address, ushort port, string password, int clientCount = 6, ILoggerGetter? loggerGetter = null) : base(loggerGetter)
        {
            ArgumentNullException.ThrowIfNull(address, nameof(address));
            ArgumentException.ThrowIfNullOrEmpty(password, nameof(password));
            ThrowHelper.ArgumentOutOfMin(0, clientCount, nameof(clientCount));

            _loggerGetter = loggerGetter;
            _address = address;
            _port = port;
            _password = password;
            _clientCount = clientCount;
            _clients = new();
            _synchronized = new();
            _index = 0;
            _id = -1;

            IsConnected = false;
        }

        private readonly ILoggerGetter? _loggerGetter;

        private readonly IPAddress _address;

        private readonly ushort _port;

        private readonly string _password;

        private readonly int _clientCount;

        private readonly List<RconClient> _clients;

        private readonly Synchronized _synchronized;

        private int _index;

        private int _id;

        public bool IsConnected { get; private set; }

        protected override void Run()
        {
            Task[] tasks = new Task[_clientCount];
            for (int i = 0; i < _clientCount; i++)
            {
                RconClient client = new(_address, _port, _password, _loggerGetter);
                client.Start("RconClient Thread #" + i);
                _clients.Add(client);
                tasks[i] = client.WaitForStopAsync();
            }

            Task.WaitAll(tasks);
        }

        public override void Stop()
        {
            foreach (var client in _clients)
                client.Stop();
            base.Stop();
        }

        protected override void DisposeUnmanaged()
        {
            foreach (var client in _clients)
                client.Dispose();
            _clients.Clear();
        }

        public void SendOnewayCommand(string command)
        {
            ArgumentException.ThrowIfNullOrEmpty(command, nameof(command));

            byte[] packet = ToPacket(GetNextIndex(), 2, command);
            _synchronized.Invoke(() => _clients[GetNextIndex()].SendPacket(packet));
        }

        public async Task SendOnewayCommandAsync(string command)
        {
            ArgumentException.ThrowIfNullOrEmpty(command, nameof(command));

            byte[] packet = ToPacket(GetNextIndex(), 2, command);
            await _synchronized.InvokeAsync(() => _clients[GetNextIndex()].SendPacketAsync(packet));
        }

        public void SendOnewayBatchCommand(IList<string> commands)
        {
            ArgumentNullException.ThrowIfNull(commands, nameof(commands));

            ConcurrentBag<byte[]> packets = ToPacketBag(commands);
            _synchronized.Invoke(() => Task.WaitAll(HandleAllCommand(packets)));
        }

        public async Task SendOnewayBatchCommandAsync(IList<string> commands)
        {
            ArgumentNullException.ThrowIfNull(commands, nameof(commands));

            ConcurrentBag<byte[]> packets = await Task.Run(() => ToPacketBag(commands)).ConfigureAwait(false);
            await _synchronized.InvokeAsync(() => Task.WhenAll(HandleAllCommand(packets)));
        }

        public async Task SendOnewayDelayBatchCommandAsync(IList<string> commands, Task? delay)
        {
            ArgumentNullException.ThrowIfNull(commands, nameof(commands));

            ConcurrentBag<byte[]> packets = await Task.Run(() => ToPacketBag(commands)).ConfigureAwait(false);

            if (delay is not null)
                await delay.ConfigureAwait(false);

            await _synchronized.InvokeAsync(() => Task.WhenAll(HandleAllCommand(packets)));
        }

        public void SendOnewayBatchSetBlock(IList<WorldBlock> arguments)
        {
            ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

            ConcurrentBag<byte[]> packets = ToPacketBag(arguments);
            _synchronized.Invoke(() => Task.WaitAll(HandleAllCommand(packets)));
        }

        public async Task SendOnewayBatchSetBlockAsync(IList<WorldBlock> arguments)
        {
            ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

            ConcurrentBag<byte[]> packets = await Task.Run(() => ToPacketBag(arguments)).ConfigureAwait(false);
            await _synchronized.InvokeAsync(() => Task.WhenAll(HandleAllCommand(packets)));
        }

        public async Task SendOnewayDelayBatchSetBlockAsync(IList<WorldBlock> arguments, Task? delay)
        {
            ArgumentNullException.ThrowIfNull(arguments, nameof(arguments));

            ConcurrentBag<byte[]> packets = await Task.Run(() => ToPacketBag(arguments)).ConfigureAwait(false);

            if (delay is not null)
                await delay.ConfigureAwait(false);

            await _synchronized.InvokeAsync(() => Task.WhenAll(HandleAllCommand(packets)));
        }

        public TimeSpan Ping()
        {
            long start = Stopwatch.GetTimestamp();
            _clients[GetNextIndex()].SendPacket(ToPacket(GetNextID(), 2, "time query gametime"));
            return Stopwatch.GetElapsedTime(start);
        }

        public async Task<TimeSpan> PingAsync()
        {
            long start = Stopwatch.GetTimestamp();
            await _clients[GetNextIndex()].SendPacketAsync(ToPacket(GetNextID(), 2, "time query gametime"));
            return Stopwatch.GetElapsedTime(start);
        }

        private int GetNextID()
        {
            return Interlocked.Decrement(ref _id);
        }

        private int GetNextIndex()
        {
            _index++;
            if (_index >= _clients.Count)
                _index = 0;
            return _index;
        }

        private static byte[] ToPacket(int id, int type, string body)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(body + "\0");
            int bodyLength = bytes.Length;

            using var packet = new MemoryStream(12 + bodyLength);
            packet.Write(BitConverter.GetBytes(9 + bodyLength), 0, 4);
            packet.Write(BitConverter.GetBytes(id), 0, 4);
            packet.Write(BitConverter.GetBytes(type), 0, 4);
            packet.Write(bytes, 0, bodyLength);
            packet.Write([0], 0, 1);

            return packet.ToArray();
        }

        private ConcurrentBag<byte[]> ToPacketBag(IList<string> commands)
        {
            ConcurrentBag<byte[]> result = new();
            ParallelLoopResult parallelLoopResult = Parallel.ForEach(commands, command =>
            {
                result.Add(ToPacket(GetNextID(), 2, command));
            });

            return result;
        }

        private ConcurrentBag<byte[]> ToPacketBag(IList<WorldBlock> blocks)
        {
            ConcurrentBag<byte[]> result = new();
            ParallelLoopResult parallelLoopResult = Parallel.ForEach(blocks, block =>
            {
                result.Add(ToPacket(GetNextID(), 2, block.ToSetBlockCommand()));
            });

            return result;
        }

        private Task[] HandleAllCommand(ConcurrentBag<byte[]> packets)
        {
            foreach (var packet in packets)
                _clients[GetNextIndex()].EnqueuePacket(packet);

            Task[] tasks = new Task[_clients.Count];
            for (int i = 0; i < _clients.Count; i++)
                tasks[i] = _clients[i].SendQueuePacketAsync();

            return tasks;
        }

        private class RconClient : UnmanagedRunnable
        {
            public RconClient(IPAddress address, ushort port, string password, ILoggerGetter? loggerGetter = null) : base(loggerGetter)
            {
                ArgumentNullException.ThrowIfNull(address, nameof(address));
                ArgumentException.ThrowIfNullOrEmpty(password, nameof(password));

                _client = new();
                _client.Connect(address, port);
                _stream = _client.GetStream();
                _buffer = new byte[4096];
                _commands = new();
                _send = new(0);
                _done = new(0);
                SendPacket(ToPacket(0, 3, password));
            }

            private readonly TcpClient _client;

            private readonly NetworkStream _stream;

            private readonly byte[] _buffer;

            private readonly Queue<byte[]> _commands;

            private readonly SemaphoreSlim _send;

            private readonly SemaphoreSlim _done;

            protected override void Run()
            {
                while (IsRunning)
                {
                    if (!_send.Wait(100))
                        continue;

                    try
                    {
                        while (_commands.Count > 0)
                            SendPacket(_commands.Dequeue());
                    }
                    catch (IOException ioIOException) when (ioIOException.InnerException is SocketException socketException)
                    {
                        if (IsRunning)
                            Logger?.Error($"RCON因为网络错误导致连接中断（{(int)socketException.SocketErrorCode}）：{socketException.Message}");
                        break;
                    }

                    _done.Release();
                }
            }

            public override void Stop()
            {
                _client.Close();
                base.Stop();
            }

            protected override void DisposeUnmanaged()
            {
                _client.Dispose();
                _send.Dispose();
                _done.Dispose();
            }

            public void EnqueuePacket(byte[] command)
            {
                _commands.Enqueue(command);
            }

            public void SendPacket(byte[] command)
            {
                _stream.Write(command);
                _stream.Read(_buffer);
            }

            public async Task SendPacketAsync(byte[] packet)
            {
                await _stream.WriteAsync(packet);
                await _stream.ReadAsync(_buffer);
            }

            public async Task SendQueuePacketAsync()
            {
                _send.Release();
                await _done.WaitAsync();
            }
        }
    }
}
