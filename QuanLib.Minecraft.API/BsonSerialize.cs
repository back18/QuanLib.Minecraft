using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API
{
    public abstract class BsonSerialize : ISerializable
    {
        public virtual byte[] Serialize()
        {
            return this.SerializeBson();
        }
    }
}
