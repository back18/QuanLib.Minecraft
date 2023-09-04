using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API
{
    public abstract class JsonData : StringSerialize
    {
        public override byte[] Serialize()
        {
            return Encoding.GetBytes(JsonConvert.SerializeObject(this));
        }
    }
}
