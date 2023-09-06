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
            if (responsePacket is null)
                throw new ArgumentNullException(nameof(responsePacket));

            return responsePacket.Data.DeserializeBson<ResponseData>();
        }

        public static async Task<string[]> SendBatchCommandAsync(this MinecraftApiClient client, string[] commands)
        {
            RequestPacket request = CreateRequestPacket(commands, client.GetNextID(), true);
            ResponsePacket response = await client.SendRequestPacket(request);
            response.ValidateStatusCode();
            return ParseResponsePacket(response).Results ?? Array.Empty<string>();
        }

        public class RequestData : BsonSerialize
        {
            public RequestData()
            {
            }

            public RequestData(string[] commands)
            {
                Commands = commands ?? throw new ArgumentNullException(nameof(commands));
            }

            public string[]? Commands { get; set; }
        }

        public class ResponseData : BsonSerialize
        {
            public ResponseData() { }

            public ResponseData(string[] results)
            {
                Results = results ?? throw new ArgumentNullException(nameof(results));
            }

            public string[]? Results { get; set; }
        }
    }
}
