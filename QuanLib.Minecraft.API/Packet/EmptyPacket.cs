using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API.Packet
{
    public static class EmptyPacket
    {
        public static RequestPacket CreateRequestPacket(int id)
        {
            return new(PacketKey.Empty, PacketType.Empty, Array.Empty<byte>(), id, false);
        }
    }
}
