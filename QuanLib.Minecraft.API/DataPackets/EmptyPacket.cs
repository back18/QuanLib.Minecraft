using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API.DataPackets
{
    public class EmptyPacket : DataPacket
    {
        public EmptyPacket(int id, bool needResponse) : base(DataPacketKey.Empty, DataPacketType.Empty, Array.Empty<byte>(), id, needResponse)
        {
        }
    }
}
