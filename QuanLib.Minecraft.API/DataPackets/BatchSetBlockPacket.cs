using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API.DataPackets
{
    public class BatchSetBlockPacket : DataPacket
    {
        public BatchSetBlockPacket(IEnumerable<ISetBlockArgument> arguments, int id, bool needResponse = false) : base(DataPacketKey.BatchSetBlock, DataPacketType.BSON, ToData(arguments), id, needResponse)
        {
        }

        private static byte[] ToData(IEnumerable<ISetBlockArgument> arguments)
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

            return new BatchSetBlock()
            {
                Palette = palette.ToArray(),
                Data = data.ToArray()
            }.ToBsonBytes();
        }

        public class BatchSetBlock
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
            public string[] Palette { get; set; }

            public int[] Data { get; set; }
        }
    }
}
