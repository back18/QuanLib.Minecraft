using QuanLib.Minecraft.API.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API.Event
{
    public class RequestPacketEventArgs : EventArgs
    {
        public RequestPacketEventArgs(RequestPacket requestPacket)
        {
            RequestPacket = requestPacket;
        }

        public RequestPacket RequestPacket { get;}
    }
}
