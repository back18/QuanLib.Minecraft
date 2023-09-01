using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API.DataPackets
{
    public class CommandPacket : DataPacket
    {
        public CommandPacket(string command, int id, bool needResponse = true) : base(DataPacketKey.Command, DataPacketType.String, ToData(command), id, needResponse)
        {
        }

        private static byte[] ToData(string command)
        {
            if (string.IsNullOrEmpty(command))
                throw new ArgumentException($"“{nameof(command)}”不能为 null 或空。", nameof(command));

            return Encoding.UTF8.GetBytes(command);
        }
    }
}
