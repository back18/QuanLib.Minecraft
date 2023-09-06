using QuanLib.Minecraft.API.Packet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API
{
    public class NetworkTask
    {
        public NetworkTask(RequestPacket request, Task send)
        {
            _request = request ?? throw new ArgumentNullException(nameof(request));
            _send = send ?? throw new ArgumentNullException(nameof(send));

            _semaphore = new(0);
            State = NetworkTaskState.Sending;

            _receive = _send.ContinueWith(async (task) =>
            {
                State = NetworkTaskState.Receiving;
                int millisecondsTimeout = 30 * 1000;
                Stopwatch stopwatch = Stopwatch.StartNew();
                await _semaphore.WaitAsync(millisecondsTimeout);
                stopwatch.Stop();
                if (stopwatch.ElapsedMilliseconds >= millisecondsTimeout)
                    State = NetworkTaskState.Timeout;
            });

            if (!_request.NeedResponse)
            {
                _semaphore.Release();
                _receive.ContinueWith((task) =>
                {
                    State = NetworkTaskState.Completed;
                });
            }
        }

        private readonly SemaphoreSlim _semaphore;

        private readonly Task _send;

        private readonly Task _receive;

        private readonly RequestPacket _request;

        private ResponsePacket? _response;

        public NetworkTaskState State { get; private set; }

        public bool IsCompleted => State is NetworkTaskState.Completed or NetworkTaskState.Timeout;

        public int DataPacketID => _request.ID;

        internal void Complete(ResponsePacket response)
        {
            if (response is null)
                throw new ArgumentNullException(nameof(response));
            if (response.ID != _request.ID)
                throw new InvalidOperationException("请求数据包与响应数据包的ID不一致");

            if (State != NetworkTaskState.Sending || State != NetworkTaskState.Receiving)
                return;

            _response = response;
            _semaphore.Release();
            _receive.ContinueWith((task) =>
            {
                State = NetworkTaskState.Completed;
            });
        }

        public async Task<ResponsePacket?> WaitForCompleteAsync()
        {
            await _send;
            await _receive;
            return _response;
        }
    }
}
