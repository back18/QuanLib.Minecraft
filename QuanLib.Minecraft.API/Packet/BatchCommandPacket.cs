using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API.Packet
{
    public static class BatchCommandPacket
    {
        public static RequestPacket CreateRequestPacket(string[] commands, int id, bool needResponse = true)
        {
            return new(PacketKey.BatchCommand, PacketType.BSON, new RequestData(commands).Serialize(), id, needResponse);
        }

        public static ResponseData ParseResponsePacket(ResponsePacket responsePacket)
        {
            ArgumentNullException.ThrowIfNull(responsePacket, nameof(responsePacket));

            return responsePacket.Data.DeserializeBson<ResponseData>();
        }

        public static async Task<string[]> SendBatchCommandAsync(this McapiClient client, string[] commands)
        {
            RequestPacket request = CreateRequestPacket(commands, client.GetNextID(), true);
            ResponsePacket response = await client.SendRequestPacketAsync(request);
            response.ValidateStatusCode();
            return ParseResponsePacket(response).Results ?? Array.Empty<string>();
        }

        public static async Task SendOnewayBatchCommandAsync(this McapiClient client, string[] commands)
        {
            RequestPacket request = CreateRequestPacket(commands, client.GetNextID(), false);
            await client.SendRequestPacketAsync(request);
        }

        public class RequestData : ISerializable
        {
            public RequestData()
            {
            }

            public RequestData(string[] commands)
            {
                ArgumentNullException.ThrowIfNull(commands, nameof(commands));

                Commands = commands;
            }

            public string[]? Commands { get; set; }

            public byte[] Serialize()
            {
                using MemoryStream stream = new();
                BsonSerializer.Serialize(new BsonBinaryWriter(stream), this);
                return stream.ToArray();
            }
        }

        public class ResponseData
        {
            public ResponseData() { }

            public ResponseData(string[] results)
            {
                ArgumentNullException.ThrowIfNull(results, nameof(results));

                Results = results;
            }

            public string[]? Results { get; set; }
        }
    }
}
