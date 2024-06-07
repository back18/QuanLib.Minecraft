using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API.Packet
{
    public abstract class DataPacket : ISerializable
    {
        protected DataPacket(string type, byte[] data, int id)
        {
            ArgumentException.ThrowIfNullOrEmpty(type, nameof(type));
            ArgumentNullException.ThrowIfNull(data, nameof(data));

            Type = type;
            Data = data;
            ID = id;
        }

        protected DataPacket(ModelBase model)
        {
            ArgumentNullException.ThrowIfNull(model, nameof(model));
            ValidationHelper.Validate(model, "数据包");

            Type = model.Type;
            Data = model.Data;
            ID = model.ID;
        }

        public string Type { get; }

        public byte[] Data { get; }

        public int ID { get; }

        public abstract byte[] Serialize();

        protected static byte[] Serialize<T>(T model) where T : ModelBase
        {
            ArgumentNullException.ThrowIfNull(model, nameof(model));

            using MemoryStream stream = new();
            BsonSerializer.Serialize(new BsonBinaryWriter(stream), model);
            return stream.ToArray();
        }

        protected static bool TryDeserialize<T>(byte[] bytes, [MaybeNullWhen(false)] out T result) where T : ModelBase
        {
            try
            {
                result = BsonSerializer.Deserialize<T>(bytes);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        public abstract class ModelBase
        {
            [Required(ErrorMessage = "Type参数缺失")]
            public required string Type { get; set; }

            [Required(ErrorMessage = "Data参数缺失")]
            public required byte[] Data { get; set; }

            [Required(ErrorMessage = "ID参数缺失")]
            public required int ID { get; set; }
        }
    }
}
