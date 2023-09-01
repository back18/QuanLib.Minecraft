using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLib.Minecraft.API.DataPackets;

namespace QuanLib.Minecraft.API
{
    public static class DataPacketHelper
    {
        private static int _id = -1;

        private static int GetNextID()
        {
            return Interlocked.Decrement(ref _id);
        }
    }
}
