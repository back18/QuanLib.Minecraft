using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API
{
    public interface ISerializable
    {
        public byte[] Serialize();
    }
}
