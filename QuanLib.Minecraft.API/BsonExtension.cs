using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API
{
    public static class BsonExtension
    {
        public static byte[] ToBsonBytes(this object source)
        {
            using MemoryStream stream = new();
            BsonSerializer.Serialize(new BsonBinaryWriter(stream), source);
            return stream.ToArray();
        }
    }
}
