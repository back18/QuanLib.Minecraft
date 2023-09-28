using log4net.Core;
using MongoDB.Bson.Serialization;
using QuanLib.Core;
using QuanLib.Core.ExceptionHelper;
using QuanLib.Minecraft.API.Event;
using QuanLib.Minecraft.API.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API
{
    public class McapiClient : UnmanagedRunnable
    {
        public McapiClient(IPAddress address, ushort port, Func<Type, LogImpl> logger) : base(logger)
        {
            if (address is null)
                throw new ArgumentNullException(nameof(address));

            _address = address;
            _port = port;
            _packetid = 0;
            _tasks = new();
            _client = new();

            ReceivedPacket += OnReceivedPacket;
            Connected += OnConnected;
        }
 
        private readonly IPAddress _address;

        private readonly ushort _port;

        private int _packetid;

        private readonly Dictionary<int, NetworkTask> _tasks;

        private readonly TcpClient _client;

        public event EventHandler<McapiClient, ResponsePacketEventArgs> ReceivedPacket;

        public event EventHandler<McapiClient, EventArgs> Connected;

        protected virtual void OnReceivedPacket(McapiClient sender, ResponsePacketEventArgs e)
        {
            if (_tasks.TryGetValue(e.ResponsePacket.ID, out var task))
            {
                task.Receive(e.ResponsePacket);
                _tasks.Remove(e.ResponsePacket.ID);
            }
            else
            {

            }
        }

        protected virtual void OnConnected(McapiClient sender, EventArgs e) { }

        protected override void Run()
        {
            _client.Connect(_address, _port);
            Connected.Invoke(this, EventArgs.Empty);
            NetworkStream stream = _client.GetStream();
            byte[] buffer = new byte[4096];
            MemoryStream cache = new();
            int total = buffer.Length;
            int current = 0;
            bool initial = true;
            stream.ReadTimeout = Timeout.Infinite;
            while (IsRuning)
            {
                int length = stream.Read(buffer, 0, Math.Min(total - current, buffer.Length));

                current += length;
                if (initial)
                {
                    stream.ReadTimeout = 30 * 1000;

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
                stream.ReadTimeout = Timeout.Infinite;
            }
        }

        protected override void DisposeUnmanaged()
        {
            _client.Dispose();
        }

        public async Task<ResponsePacket> SendRequestPacketAsync(RequestPacket request)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));
            if (!request.NeedResponse)
                throw new ArgumentException("request.NeedResponse is false", nameof(request));

            NetworkTask task = new(_client, request);
            _tasks.Add(request.ID, task);
            task.Send();
            ResponsePacket? response = await task.WaitForCompleteAsync();
            if (response is null)
            {
                if (task.State == NetworkTaskState.Timeout)
                    throw new InvalidOperationException("MCAPI请求超时");
                else
                    throw new InvalidOperationException("MCAPI数据包发送或接收失败");
            }
            return response;
        }

        public async Task SendOnewayRequestPacketAsync(RequestPacket request)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));
            if (request.NeedResponse)
                throw new ArgumentException("request.NeedResponse is true", nameof(request));

            await _client.GetStream().WriteAsync(request.Serialize());
        }

        public async Task<bool> LoginAsync(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException($"“{nameof(password)}”不能为 null 或空。", nameof(password));

            SemaphoreSlim semaphore = new(0);
            Connected += Release;
            if (!_client.Connected)
                await semaphore.WaitAsync();
            Connected -= Release;

            var result = await this.SendLoginAsync(password);
            return result.IsSuccessful ?? false;

            void Release(McapiClient sender, EventArgs e) => semaphore.Release();
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
    }
}
