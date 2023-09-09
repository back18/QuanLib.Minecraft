using MongoDB.Bson.Serialization;
using QuanLib.Core;
using QuanLib.Core.ExceptionHelper;
using QuanLib.Minecraft.API.Event;
using QuanLib.Minecraft.API.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API
{
    public class MinecraftApiClient : ISwitchable, IDisposable
    {
        public MinecraftApiClient(string address, ushort port, string password)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentException($"“{nameof(address)}”不能为 null 或空。", nameof(address));
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException($"“{nameof(password)}”不能为 null 或空。", nameof(password));

            _address = address;
            _port = port;
            _password = password;
            _packetid = 0;
            _tasks = new();
            _client = new();
            _runing = false;

            ReceivedPacket += OnReceivedPacket;
        }

        private readonly string _address;

        private readonly ushort _port;

        private readonly string _password;

        private int _packetid;

        private readonly Dictionary<int, NetworkTask> _tasks;

        private readonly TcpClient _client;

        private NetworkStream? _stream;

        private bool _runing;

        public bool Runing => _runing;

        public event EventHandler<MinecraftApiClient, ResponsePacketEventArgs> ReceivedPacket;

        protected virtual void OnReceivedPacket(MinecraftApiClient sender, ResponsePacketEventArgs e)
        {
            if (_tasks.TryGetValue(e.ResponsePacket.ID, out var task))
            {
                task.Complete(e.ResponsePacket);
                _tasks.Remove(e.ResponsePacket.ID);
            }
        }

        public void Start()
        {
            if (_runing)
                return;
            _runing = true;

            _client.Connect(_address, _port);
            _stream = _client.GetStream();
            StartReadStream();

            _runing = false;
            if (_stream is not null)
                _stream.ReadTimeout = 1;
        }

        public void Stop()
        {
            _runing = false;
        }

        public async Task<ResponsePacket> SendRequestPacket(RequestPacket request)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));
            if (_stream is null)
                throw new InvalidOperationException("TCP未连接");

            Task send = _stream.WriteAsync(request.Serialize()).AsTask();
            NetworkTask task = new(request, send);
            _tasks.Add(request.ID, task);
            ResponsePacket? response = await task.WaitForCompleteAsync() ?? throw new InvalidOperationException("数据包发送或接收失败");
            return response;
        }

        private void StartReadStream()
        {
            if (_stream is null)
                throw new InvalidOperationException("TCP未连接");

            try
            {
                byte[] buffer = new byte[4096];
                MemoryStream cache = new();
                int total = buffer.Length;
                int current = 0;
                bool initial = true;
                _stream.ReadTimeout = Timeout.Infinite;
                while (_runing)
                {
                    int length = _stream.Read(buffer, 0, Math.Min(total - current, buffer.Length));

                    current += length;
                    if (initial)
                    {
                        _stream.ReadTimeout = 30 * 1000;

                        if (current < 4)
                            continue;

                        total = BitConverter.ToInt32(buffer, 0);
                        if (total < 4)
                            throw new IOException($"读取数据包时出现错误：数据包长度标识不能小于4");

                        initial = false;
                    }

                    cache.Write(buffer, 0, length);

                    if (current < total)
                        continue;

                    HandleDataPacket(cache.ToArray());

                    cache.Dispose();
                    cache = new();
                    total = buffer.Length;
                    current = 0;
                    initial = true;
                    _stream.ReadTimeout = Timeout.Infinite;
                }
            }
            catch
            {
                _runing = false;
            }
        }

        private void HandleDataPacket(byte[] bytes)
        {
            if (ResponsePacket.TryDeserialize(bytes, out var response))
            {
                ReceivedPacket.Invoke(this, new(response));
            }
        }

        public int GetNextID()
        {
            return Interlocked.Decrement(ref _packetid);
        }

        public void Dispose()
        {
            _stream?.Dispose();
            _client.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
