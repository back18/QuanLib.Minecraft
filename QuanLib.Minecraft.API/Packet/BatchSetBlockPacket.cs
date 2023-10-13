using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API.Packet
{
    public static class BatchSetBlockPacket
    {
        public static RequestPacket CreateRequestPacket(IEnumerable<ISetBlockArgument> arguments, int id, bool needResponse = true)
        {
            return new(PacketKey.BatchSetBlock, PacketType.BSON, new RequestData(arguments).Serialize(), id, needResponse);
        }

        public static ResponseData ParseResponsePacket(ResponsePacket responsePacket)
        {
            if (responsePacket is null)
                throw new ArgumentNullException(nameof(responsePacket));

            return responsePacket.Data.DeserializeBson<ResponseData>();
        }

        public static async Task<ResponseData> SendBatchSetBlockAsync(this McapiClient client, IEnumerable<ISetBlockArgument> arguments)
        {
            RequestPacket request = CreateRequestPacket(arguments, client.GetNextID(), true);
            ResponsePacket response = await client.SendRequestPacketAsync(request);
            response.ValidateStatusCode();
            return ParseResponsePacket(response);
        }

        public static async Task SendOnewayBatchSetBlockAsync(this McapiClient client, IEnumerable<ISetBlockArgument> arguments)
        {
            RequestPacket request = CreateRequestPacket(arguments, client.GetNextID(), false);
            await client.SendOnewayRequestPacketAsync(request);
        }

        public class RequestData : ISerializable
        {
            public RequestData() { }

            public RequestData(IEnumerable<ISetBlockArgument> arguments)
            {
                if (arguments is null)
                    throw new ArgumentNullException(nameof(arguments));

                List<string> palette = new();
                List<int> data = new(arguments.Count() * 4);
                foreach (ISetBlockArgument argument in arguments)
                {
                    int index = palette.IndexOf(argument.BlockID);
                    if (index == -1)
                    {
                        palette.Add(argument.BlockID);
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
