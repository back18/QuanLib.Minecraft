using QuanLib.Core;
using QuanLib.Core.Events;
using QuanLib.Minecraft.API.Packet;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API
{
    public class McapiClient : UnmanagedRunnable
    {
        public McapiClient(IPAddress address, ushort port, ILoggerProvider? loggerProvider = null) : base(loggerProvider)
        {
            ArgumentNullException.ThrowIfNull(address, nameof(address));

            _address = address;
            _port = port;
            _packetid = 0;
            _tasks = new();
            _client = new();
            _synchronized = new();
            _stopping = false;

            ReceivedPacket += OnReceivedPacket;
            Connected += OnConnected;
        }

        private readonly IPAddress _address;

        private readonly ushort _port;

        private int _packetid;

        private readonly ConcurrentDictionary<int, NetworkTask> _tasks;

        private readonly TcpClient _client;

        private readonly Synchronized _synchronized;

        private bool _stopping;

        public event EventHandler<McapiClient, EventArgs<ResponsePacket>> ReceivedPacket;

        public event EventHandler<McapiClient, EventArgs> Connected;

        protected virtual void OnReceivedPacket(McapiClient sender, EventArgs<ResponsePacket> e)
        {
            if (_tasks.TryGetValue(e.Argument.ID, out var task))
            {
                task.Completed(e.Argument);
                _tasks.TryRemove(e.Argument.ID, out _);
            }
            else
            {
                Logger?.Debug("未知的响应包ID：" + e.Argument.ID);
            }
        }

        protected virtual void OnConnected(McapiClient sender, EventArgs e) { }

        protected override void Run()
        {
            using MemoryStream memoryStream = new();
            byte[] buffer = new byte[4096];
            int total = 0;
            int current = 0;

            try
            {
                _client.Connect(_address, _port);
            }
            catch (SocketException socketException)
            {
                Logger?.Error($"MCAPI因为网络错误导致连接失败（{(int)socketException.SocketErrorCode}）：{socketException.Message}");
                return;
            }

            NetworkStream networkStream = _client.GetStream();
            networkStream.ReadTimeout = Timeout.Infinite;
            Connected.Invoke(this, EventArgs.Empty);

            while (IsRunning)
            {
                int length = total == 0 ? 4 : Math.Min(total - current, buffer.Length);
                int readLength;

                try
                {
                    readLength = networkStream.Read(buffer, 0, length);
                }
                catch (IOException ioIOException) when (ioIOException.InnerException is SocketException socketException)
                {
                    if (!_stopping)
                        Logger?.Error($"MCAPI因为网络错误导致连接中断（{(int)socketException.SocketErrorCode}）：{socketException.Message}");
                    NoticeTaskFailed(socketException);
                    break;
                }
                catch (Exception ex)
                {
                    NoticeTaskFailed(ex);
                    throw;
                }

                if (readLength == 0)
                {
                    Logger?.Info("MCAPI已正常关闭连接");
                    break;
                }

                current += readLength;
                if (total == 0)
                {
                    networkStream.ReadTimeout = 30 * 1000;

                    if (current < 4)
                        continue;

                    total = BitConverter.ToInt32(buffer, 0);
                    if (total < 4)
                    {
                        IOException exception = new($"读取到数据包长度 {total} 小于最小长度 4 字节，已无法确保网络流的正确性");
                        NoticeTaskFailed(exception);
                        throw exception;
                    }
                }

                memoryStream.Write(buffer, 0, readLength);

                if (current < total)
                    continue;

                byte[] DataPacket = new byte[total];
                memoryStream.Seek(0, SeekOrigin.Begin);
                memoryStream.Read(DataPacket, 0, total);
                memoryStream.Seek(0, SeekOrigin.Begin);

                HandleResponsePacket(DataPacket);

                total = 0;
                current = 0;
                networkStream.ReadTimeout = Timeout.Infinite;
            }
        }

        public override void Stop()
        {
            _stopping = true;

            if (_client.Connected)
            {
                int i = 0;

                do
                {
                    Thread.Sleep(1);
                    i++;
                } while (!_tasks.IsEmpty && i < 100);

                _client.Close();
            }

            if (!_tasks.IsEmpty)
                NoticeTaskFailed(new IOException("MCAPI已断开连接"));

            base.Stop();
        }

        protected override void DisposeUnmanaged()
        {
            _client.Dispose();
        }

        public async Task<ResponsePacket> SendRequestPacketAsync(RequestPacket request)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));
            if (!request.NeedResponse)
                throw new ArgumentException("request.NeedResponse is false", nameof(request));
            CheckConnection();

            NetworkTask task = new(ThreadSafeWriteAsync, request);
            _tasks.TryAdd(request.ID, task);
            task.Send();

            ResponsePacket? response = await task.WaitForCompleteAsync();
            if (response is null)
            {
                switch (task.State)
                {
                    case NetworkTaskState.Failed:
                        _tasks.TryRemove(request.ID, out _);
                        throw new IOException("MCAPI请求失败", task.Exception);
                    case NetworkTaskState.Timeout:
                        _tasks.TryRemove(request.ID, out _);
                        throw new IOException("MCAPI请求超时");
                    default:
                        throw new IOException("MCAPI响应包丢失");
                }
            }

            return response;
        }

        public ValueTask SendOnewayRequestPacketAsync(RequestPacket request)
        {
            ArgumentNullException.ThrowIfNull(request, nameof(request));
            if (request.NeedResponse)
                throw new ArgumentException("request.NeedResponse is true", nameof(request));
            CheckConnection();

            byte[] datapacket = request.Serialize();
            return ThreadSafeWriteAsync(datapacket);
        }

        public async Task<LoginResult> LoginAsync(string password)
        {
            ArgumentException.ThrowIfNullOrEmpty(password, nameof(password));

            TaskSemaphore semaphore = new();
            Connected += Release;
            if (!_client.Connected)
                await semaphore.WaitAsync().ConfigureAwait(false);
            Connected -= Release;

            var result = await this.SendLoginAsync(password).ConfigureAwait(false);
            return new LoginResult(result.IsSuccessful ?? false, result.Message ?? string.Empty);

            void Release(McapiClient sender, EventArgs e) => semaphore.Release();
        }

        private void HandleResponsePacket(byte[] bytes)
        {
            if (ResponsePacket.TryDeserialize(bytes, out var response))
            {
                ReceivedPacket.Invoke(this, new(response));
            }
            else
            {
                Logger?.Debug($"无法解析响应数据包，已丢弃，长度为{bytes.Length}");
            }
        }

        public int GetNextID()
        {
            return Interlocked.Increment(ref _packetid);
        }

        internal ValueTask ThreadSafeWriteAsync(byte[] datapacket)
        {
            ArgumentNullException.ThrowIfNull(datapacket, nameof(datapacket));

            return _synchronized.InvokeAsync(() => _client.GetStream().WriteAsync(datapacket));
        }

        private void CheckConnection()
        {
            if (_stopping || !IsRunning || !_client.Connected)
                throw new IOException("MCAPI已断开连接，无法继续发送数据");
        }

        private void NoticeTaskFailed(Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception, nameof(exception));

            _stopping = true;
            Thread.Sleep(1);

            foreach (NetworkTask task in _tasks.Values)
                task.Failed(exception);
            _tasks.Clear();
        }

        public record LoginResult(bool IsSuccessful, string Message);
    }
}
