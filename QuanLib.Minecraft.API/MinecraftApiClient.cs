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

            try
            {
                StartReadStream();
            }
            catch
            {

            }
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public async Task<ResponsePacket> SendPacke(RequestPacket request)
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

            byte[] buffer = new byte[4096];
            MemoryStream memoryStream = new();
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

                    total = BitConverter.ToInt32(new byte[] { buffer[3], buffer[2], buffer[1], buffer[0] });
                    if (total < 4)
                        throw new InvalidOperationException($"数据包读取错误，“{total}”不能小于4");

                    memoryStream.Write(buffer, 4, length - 4);

                    initial = false;
                }
                else
                {
                    memoryStream.Write(buffer, 0, length);
                }

                if (current < total)
                    continue;

                HandleDataPacket(memoryStream.ToArray());

                memoryStream.Dispose();
                memoryStream = new();
                total = buffer.Length;
                current = 0;
                initial = true;
                _stream.ReadTimeout = Timeout.Infinite;
            }
        }

        private void HandleDataPacket(byte[] bytes)
        {
            try
            {
                ResponsePacket.ResponseModel model = BsonSerializer.Deserialize<ResponsePacket.ResponseModel>(bytes);
                ResponsePacket packet = new(model);
                ReceivedPacket.Invoke(this, new(packet));
            }
            catch
            {

            }
        }

        public int GetNextID()
        {
            return Interlocked.Decrement(ref _packetid);
        }

        public void Dispose()
        {
            _client.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
