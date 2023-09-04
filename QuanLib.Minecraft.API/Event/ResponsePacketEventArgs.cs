using QuanLib.Minecraft.API.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API.Event
{
    public class ResponsePacketEventArgs : EventArgs
    {
        public ResponsePacketEventArgs(ResponsePacket responsePacket)
        {
            ResponsePacket = responsePacket;
        }

        public ResponsePacket ResponsePacket { get; }
    }
}
