using QuanLib.Minecraft.API.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API.Events
{
    public class DataPacketEventArgs : EventArgs
    {
        public DataPacketEventArgs(DataPacket dataPacket)
        {
            DataPacket = dataPacket;
        }

        public DataPacket DataPacket { get; }
    }
}
