using QuanLib.Core;
using QuanLib.Minecraft.API.Packet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API
{
    public class NetworkTask
    {
        public NetworkTask(TcpClient tcpClient, RequestPacket request, Synchronized synchronized)
        {
            _sendSemaphore = new(0);
            _receiveSemaphore = new(0);
            State = NetworkTaskState.Notsent;

            _tcpClient = tcpClient ?? throw new ArgumentNullException(nameof(tcpClient));
            _request = request ?? throw new ArgumentNullException(nameof(request));
            _synchronized = synchronized ?? throw new ArgumentNullException(nameof(synchronized));
            _sendTask = WaitForSendAsync();
            _receiveTask = WaitForReceiveAsync();

            if (!_request.NeedResponse)
            {
                _receiveSemaphore.Release();
                _receiveTask.ContinueWith((task) =>
                {
                    State = NetworkTaskState.Completed;
                });
            }
        }

        private readonly SemaphoreSlim _sendSemaphore;

        private readonly SemaphoreSlim _receiveSemaphore;

        private readonly Task _sendTask;

        private readonly Task _receiveTask;

        private readonly TcpClient _tcpClient;

        private readonly RequestPacket _request;

        private ResponsePacket? _response;

        private readonly Synchronized _synchronized;

        public NetworkTaskState State { get; private set; }

        public bool IsCompleted => State is NetworkTaskState.Completed or NetworkTaskState.Timeout;

        public int DataPacketID => _request.ID;

        internal void Send()
        {
            _sendSemaphore.Release();
        }

        internal void Receive(ResponsePacket response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));
            if (response.ID != _request.ID)
                throw new InvalidOperationException("请求数据包与响应数据包的ID不一致");

            _sendTask.Wait();
            _response = response;
            _receiveSemaphore.Release();
            _receiveTask.ContinueWith((task) =>
            {
                State = NetworkTaskState.Completed;
            });
        }

        public async Task<ResponsePacket?> WaitForCompleteAsync()
        {
            await _sendTask;
            await _receiveTask;
            return _response;
        }

        private async Task WaitForSendAsync()
        {
            byte[] datapacket = _request.Serialize();
            await _sendSemaphore.WaitAsync();
            State = NetworkTaskState.Sending;
            await _synchronized.InvokeAsync(() => _tcpClient.GetStream().WriteAsync(datapacket));
        }

        private async Task WaitForReceiveAsync()
        {
            await _sendTask;
            State = NetworkTaskState.Receiving;
            int millisecondsTimeout = 30 * 1000;
            Stopwatch stopwatch = Stopwatch.StartNew();
            await _receiveSemaphore.WaitAsync(millisecondsTimeout);
            stopwatch.Stop();
            if (stopwatch.ElapsedMilliseconds >= millisecondsTimeout)
                State = NetworkTaskState.Timeout;
        }
    }
}
