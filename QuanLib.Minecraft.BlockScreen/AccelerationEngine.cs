using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public class AccelerationEngine : ISwitchable, IDisposable
    {
        public AccelerationEngine(string serverAddress, ushort eventPort, ushort dataPort)
        {
            ServerAddress = serverAddress;
            EventPort = eventPort;
            DataPort = dataPort;

            //_tcp = new();
            _data = new();
            _semaphore = new(1);

            OnTick += (obj) => { };
        }

        //private readonly TcpClient _tcp;

        private readonly TcpClient _data;

        private NetworkStream _dataStream;

        private readonly SemaphoreSlim _semaphore;

        public string ServerAddress { get; }

        public ushort EventPort { get; }

        public ushort DataPort { get; }

        public bool _runing;

        public bool Runing => _runing;

        public event Action<int> OnTick;

        public void Start()
        {
            Connect();

            _dataStream = _data.GetStream();

            //var stream = _tcp.GetStream();
            //byte[] buffer = new byte[4096];
            //while (_runing)
            //{
            //    int length = stream.Read(buffer);
            //}
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Connect()
        {
            //_tcp.Connect(ServerAddress, EventPort);
            _data.Connect(ServerAddress, DataPort);
        }

        public async Task ConnectAsync()
        {
            //await _tcp.ConnectAsync(ServerAddress, EventPort);
            await _data.ConnectAsync(ServerAddress, DataPort);
        }

        public async Task<int> SendDataAsync(byte[] bytes)
        {
            _semaphore.Wait();
            try
            {
                byte[] respond = new byte[4096];
                await _dataStream.WriteAsync(bytes);
                await _dataStream.ReadAsync(respond);
                return BitConverter.ToInt32(respond, 0);
            }
            catch
            {
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public int SendData(byte[] bytes)
        {
            _semaphore.Wait();
            try
            {
                byte[] respond = new byte[4096];
                _dataStream.Write(bytes);
                _dataStream.Read(respond);
                return BitConverter.ToInt32(respond, 0);
            }
            catch
            {
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void Dispose()
        {
            //_tcp.Dispose();
            _dataStream.Dispose();
            _data.Dispose();
            GC.SuppressFinalize(this);
        }

        public class DataPacket
        {
            private DataPacket(List<string> palette, List<int> data)
            {
                Palette = palette;
                Data = data;
            }

            public List<string> Palette { get; set; }

            public List<int> Data { get; set; }

            public static DataPacket ToDataPacket(IEnumerable<WorldPixel> pixels)
            {
                List<string> palette = new();
                List<int> datas = new(pixels.Count() * 4);
                foreach (WorldPixel pixel in pixels)
                {
                    int index = palette.IndexOf(pixel.BlockID);
                    if (index == -1)
                    {
                        palette.Add(pixel.BlockID);
                        index = palette.Count - 1;
                    }
                    datas.Add(pixel.Position.X);
                    datas.Add(pixel.Position.Y);
                    datas.Add(pixel.Position.Z);
                    datas.Add(index);
                }

                return new(palette, datas);
            }

            public byte[] ToBytes()
            {
                string palette = string.Join('\n', Palette);
                byte[] paletteBytes = Encoding.UTF8.GetBytes(palette);
                byte[] dataBytes = new byte[Data.Count * sizeof(int)];
                for (int i = 0; i < Data.Count; i++)
                {
                    byte[] bytes = BitConverter.GetBytes(Data[i]);
                    Array.Reverse(bytes);
                    bytes.CopyTo(dataBytes, i * 4);
                }

                byte[] resultBytes = new byte[8 + paletteBytes.Length + dataBytes.Length];
                byte[] resultBytesLength = BitConverter.GetBytes(resultBytes.Length);
                Array.Reverse(resultBytesLength);
                resultBytesLength.CopyTo(resultBytes, 0);
                byte[] paletteBytesLength = BitConverter.GetBytes(paletteBytes.Length);
                Array.Reverse(paletteBytesLength);
                paletteBytesLength.CopyTo(resultBytes, 4);
                paletteBytes.CopyTo(resultBytes, 8);
                dataBytes.CopyTo(resultBytes, 8 + paletteBytes.Length);
                return resultBytes;
            }
        }
    }
}
