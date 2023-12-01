using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;
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

            List<ValidationResult> results = new();
            if (!Validator.TryValidateObject(model, new(model), results, true))
            {
                StringBuilder message = new();
                message.AppendLine();
                int count = 0;
                foreach (var result in results)
                {
                    string memberName = result.MemberNames.FirstOrDefault() ?? string.Empty;
                    message.AppendLine($"[{memberName}]: {result.ErrorMessage}");
                    count++;
                }
                message.Insert(0, $"“{nameof(model)}”的参数不能为 null 或空");
                throw new ArgumentException(message.ToString().TrimEnd(), nameof(model));
            }

            Type = model.Type;
            Data = model.Data;
            ID = model.ID!.Value;
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
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

            [Required(ErrorMessage = "Type参数缺失")]
            public string Type { get; set; }

            [Required(ErrorMessage = "Data参数缺失")]
            public byte[] Data { get; set; }

            [Required(ErrorMessage = "ID参数缺失")]
            public int? ID { get; set; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        }
    }
}
