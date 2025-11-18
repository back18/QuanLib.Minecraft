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
        public NetworkTask(Func<byte[], ValueTask> writeFunc, RequestPacket request)
        {
            ArgumentNullException.ThrowIfNull(writeFunc, nameof(writeFunc));
            ArgumentNullException.ThrowIfNull(request, nameof(request));

            _sendSemaphore = new();
            _receiveSemaphore = new();
            State = NetworkTaskState.NotStarted;

            _writeFunc = writeFunc;
            _request = request;
            _receiveTask = WaitForReceiveAsync();

            if (!_request.NeedResponse)
                _receiveSemaphore.Release();
        }

        private readonly TaskSemaphore _sendSemaphore;

        private readonly TaskSemaphore _receiveSemaphore;

        private readonly Task _receiveTask;

        private readonly Func<byte[], ValueTask> _writeFunc;

        private readonly RequestPacket _request;

        private ResponsePacket? _response;

        public NetworkTaskState State { get; private set; }

        public bool IsCompleted => State is NetworkTaskState.Completed or NetworkTaskState.Failed or NetworkTaskState.Timeout;

        public bool IsFailed => State is NetworkTaskState.Failed or NetworkTaskState.Timeout;

        public int DataPacketID => _request.ID;

        public Exception? Exception { get; private set; }

        internal void Send()
        {
            _sendSemaphore.Release();
        }

        internal void Completed(ResponsePacket response)
        {
            ArgumentNullException.ThrowIfNull(response, nameof(response));
            if (response.ID != _request.ID)
                throw new InvalidOperationException("请求数据包与响应数据包的ID不一致");

            _response = response;
            _receiveSemaphore.Release();
        }

        internal void Failed(Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception, nameof(exception));

            Exception = exception;
            _receiveSemaphore.Release();
        }

        public async Task<ResponsePacket?> WaitForCompleteAsync()
        {
            await _receiveTask.ConfigureAwait(false);
            return _response;
        }

        private async Task WaitForReceiveAsync()
        {
            byte[] datapacket = _request.Serialize();
            await _sendSemaphore.WaitAsync().ConfigureAwait(false);

            try
            {
                await _writeFunc.Invoke(datapacket).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                State = NetworkTaskState.Failed;
                Exception = ex;
                return;
            }

            State = NetworkTaskState.WaitForReceive;
            if (await _receiveSemaphore.WaitAsync(30 * 1000).ConfigureAwait(false))
            {
                if (Exception is null)
                    State = NetworkTaskState.Completed;
                else
                    State = NetworkTaskState.Failed;
            }
            else
            {
                State = NetworkTaskState.Timeout;
            }
        }
    }
}
