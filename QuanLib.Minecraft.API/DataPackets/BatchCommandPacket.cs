using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API.DataPackets
{
    public class BatchCommandPacket : DataPacket
    {
        public BatchCommandPacket(string[] commands, int id, bool needResponse = true) : base(DataPacketKey.BatchCommand, DataPacketType.BSON, ToData(commands), id, needResponse)
        {
        }

        public static byte[] ToData(string[] commands)
        {
            if (commands is null)
                throw new ArgumentNullException(nameof(commands));

            return new BatchCommand()
            {
                Commands = commands
            }.ToBsonBytes();
        }

        public class BatchCommand
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
            public string[] Commands { get; set; }
        }
    }
}
