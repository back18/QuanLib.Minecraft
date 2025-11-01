using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API.Packet
{
    public static class BatchSetBlockPacket
    {
        public static RequestPacket CreateRequestPacket(IList<WorldBlock> blocks, int id, bool needResponse = true)
        {
            return new(PacketKey.BatchSetBlock, PacketType.BSON, new RequestData(blocks).Serialize(), id, needResponse);
        }

        public static ResponseData ParseResponsePacket(ResponsePacket responsePacket)
        {
            ArgumentNullException.ThrowIfNull(responsePacket, nameof(responsePacket));

            return responsePacket.Data.DeserializeBson<ResponseData>();
        }

        public static async Task<ResponseData> SendBatchSetBlockAsync(this McapiClient client, IList<WorldBlock> blocks)
        {
            RequestPacket request = await Task.Run(() => CreateRequestPacket(blocks, client.GetNextID(), true)).ConfigureAwait(false);
            ResponsePacket response = await client.SendRequestPacketAsync(request).ConfigureAwait(false);
            response.ValidateStatusCode();
            return ParseResponsePacket(response);
        }

        public static async Task<ResponseData> SendDelayBatchSetBlockAsync(this McapiClient client, IList<WorldBlock> blocks, Task? delay)
        {
            RequestPacket request = await Task.Run(() => CreateRequestPacket(blocks, client.GetNextID(), true)).ConfigureAwait(false);

            if (delay is not null)
                await delay.ConfigureAwait(false);

            ResponsePacket response = await client.SendRequestPacketAsync(request).ConfigureAwait(false);
            response.ValidateStatusCode();
            return ParseResponsePacket(response);
        }

        public static async Task SendOnewayBatchSetBlockAsync(this McapiClient client, IList<WorldBlock> blocks)
        {
            RequestPacket request = await Task.Run(() => CreateRequestPacket(blocks, client.GetNextID(), false)).ConfigureAwait(false);
            await client.SendOnewayRequestPacketAsync(request).ConfigureAwait(false);
        }

        public static async Task SendOnewayDelayBatchSetBlockAsync(this McapiClient client, IList<WorldBlock> blocks, Task? delay)
        {
            RequestPacket request = await Task.Run(() => CreateRequestPacket(blocks, client.GetNextID(), false)).ConfigureAwait(false);

            if (delay is not null)
                await delay.ConfigureAwait(false);

            await client.SendOnewayRequestPacketAsync(request).ConfigureAwait(false);
        }

        public class RequestData : ISerializable
        {
            public RequestData() { }

            public RequestData(IList<WorldBlock> blocks)
            {
                ArgumentNullException.ThrowIfNull(blocks, nameof(blocks));

                List<string> palette = [];
                List<int> data = new(blocks.Count * 4);
                foreach (WorldBlock argument in blocks)
                {
                    int index = palette.IndexOf(argument.BlockId);
                    if (index == -1)
                    {
                        palette.Add(argument.BlockId);
                        index = palette.Count - 1;
                    }
                    data.Add(argument.Position.X);
                    data.Add(argument.Position.Y);
                    data.Add(argument.Position.Z);
                    data.Add(index);
                }

                Palette = palette.ToArray();
                Data = data.ToArray();
            }

            public string[]? Palette { get; set; }

            public int[]? Data { get; set; }

            public byte[] Serialize()
            {
                using MemoryStream stream = new();
                BsonSerializer.Serialize(new BsonBinaryWriter(stream), this);
                return stream.ToArray();
            }
        }

        public class ResponseData
        {
            public int? TotalCount { get; set; }

            public int? CompletedCount { get; set; }
        }
    }
}
