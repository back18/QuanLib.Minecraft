using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QuanLib.Minecraft
{
    public class RconMax : IDisposable, IBytesCommandSender
    {
        public RconMax(string ip, ushort port, string password, int clientCount = 6)
        {
            _ip = ip;
            _port = port;
            _password = password;
            _clientCount = clientCount;
            _index = 0;
            _id = -1;
            _clients = new();
            _events = new();
        }

        private readonly string _ip;

        private readonly ushort _port;

        private readonly string _password;

        private readonly int _clientCount;

        private readonly List<RconClient> _clients;

        private readonly List<AutoResetEvent> _events;

        private int _index;

        private int _id;

        private bool _connected;

        public bool Connected => _connected;

        public void Connect()
        {
            Close();
            for (int i = 0; i < _clientCount; i++)
            {
                RconClient client = new(_ip, _port, _password);
                Task.Run(() => client.Start());
                _clients.Add(client);
                _events.Add(client._done);
            }
            _connected = true;
        }

        public void Close()
        {
            foreach (var client in _clients)
                client.Close();
            _clients.Clear();
            _events.Clear();
            _connected = false;
        }

        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }

        public void SendCommand(string command)
        {
            if (command is null)
                throw new ArgumentNullException(nameof(command));

            lock (_clients)
                _clients[0].SendCommand(CommandToBytes(command));
        }

        public void SendAllCommand(IEnumerable<string> commands)
        {
            if (commands is null)
                throw new ArgumentNullException(nameof(commands));

            lock (_clients)
            {
                foreach (var command in commands)
                {
                    _clients[GetClientIndex()].AddCommand(CommandToBytes(command));
                }

                for (int i = 0; i < _clients.Count; i++)
                    _clients[i].SendAllCommandNotWait();

                WaitHandle.WaitAll(_events.ToArray());
            }
        }

        public async Task SendCommandAsync(string command)
        {
            await Task.Run(() => SendCommand(command));
        }

        public async Task SendAllCommandAsync(IEnumerable<string> commands)
        {
            await Task.Run(() => SendAllCommand(commands));
        }

        public void SendCommand(byte[] bytesCommand)
        {
            if (bytesCommand is null)
                throw new ArgumentNullException(nameof(bytesCommand));

            lock (_clients)
                _clients[0].SendCommand(bytesCommand);
        }

        public void SendAllCommand(IEnumerable<byte[]> bytesCommands)
        {
            if (bytesCommands is null)
                throw new ArgumentNullException(nameof(bytesCommands));

            lock (_clients)
            {
                foreach (var bytesCommand in bytesCommands)
                {
                    _clients[GetClientIndex()].AddCommand(bytesCommand);
                }

                for (int i = 0; i < _clients.Count; i++)
                    _clients[i].SendAllCommandNotWait();

                WaitHandle.WaitAll(_events.ToArray());
            }
        }

        public async Task SendCommandAsync(byte[] bytesCommand)
        {
            await Task.Run(() => SendCommand(bytesCommand));
        }

        public async Task SendAllCommandAsync(IEnumerable<byte[]> bytesCommands)
        {
            await Task.Run(() => SendAllCommand(bytesCommands));
        }

        public byte[] CommandToBytes(string command)
        {
            return ToPacket(GetNextID(), 2, command);
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
            packet.Write(new byte[] { 0 }, 0, 1);

            return packet.ToArray();
        }

        private int GetNextID()
        {
            return Interlocked.Decrement(ref _id);
        }

        private int GetClientIndex()
        {
            _index++;
            if (_index >= _clients.Count)
                _index = 0;
            return _index;
        }

        private class RconClient
        {
            public RconClient(string ip, ushort port, string password)
            {
                _client = new(ip, port);
                _stream = _client.GetStream();
                _buffer = new byte[1024];
                _commands = new();
                _send = new(false);
                _done = new(false);
                _stream.Write(ToPacket(0, 3, password));
                _stream.Read(_buffer);
            }

            private readonly TcpClient _client;

            private readonly NetworkStream _stream;

            private readonly byte[] _buffer;

            private readonly Queue<byte[]> _commands;

            private readonly AutoResetEvent _send;

            public readonly AutoResetEvent _done;

            public void Close()
            {
                if (!_client.Connected)
                {
                    _stream.Close();
                    _client.Close();
                }
            }

            public void AddCommand(byte[] command)
            {
                _commands.Enqueue(command);
            }

            public void Start()
            {
                while (true)
                {
                    _send.WaitOne();

                    while (_commands.Count > 0)
                    {
                        SendCommand(_commands.Dequeue());
                    }

                    _done.Set();
                }
            }

            public void SendCommand(byte[] command)
            {
                _stream.Write(command);
                _stream.Read(_buffer);
            }

            public void SendAllCommand()
            {
                _send.Set();
                _done.WaitOne();
            }

            public void SendAllCommandNotWait()
            {
                _send.Set();
            }
        }
    }
}
