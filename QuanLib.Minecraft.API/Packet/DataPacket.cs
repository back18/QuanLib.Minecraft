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
            if (string.IsNullOrEmpty(type))
                throw new ArgumentException($"“{nameof(type)}”不能为 null 或空。", nameof(type));
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            Type = type;
            Data = data;
            ID = id;
        }

        protected DataPacket(ModelBase model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

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
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            using MemoryStream stream = new();
            BsonSerializer.Serialize(new BsonBinaryWriter(stream), model);
            byte[] length = BitConverter.GetBytes((int)stream.Length + 4);
            Array.Reverse(length);
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(length, 0, length.Length);
            return stream.ToArray();
        }

        protected static bool TryDeserialize<T>(byte[] bytes, [MaybeNullWhen(false)] out T result) where T : ModelBase
        {
            if (bytes is null || bytes.Length <= 4)
                goto err;

            try
            {
                using MemoryStream stream = new(bytes);
                stream.Seek(0, SeekOrigin.Begin);
                byte[] buffer1 = new byte[4];
                stream.Read(buffer1, 0, buffer1.Length);
                Array.Reverse(buffer1);
                int length = BitConverter.ToInt32(buffer1);
                if (length != stream.Length)
                    goto err;

                stream.Seek(4, SeekOrigin.Begin);
                byte[] buffer2 = new byte[stream.Length - 4];
                stream.Read(buffer2, 0, buffer2.Length);
                T model = BsonSerializer.Deserialize<T>(buffer2);
                result = model;
                return true;
            }
            catch
            {
                goto err;
            }

            err:
            result = null;
            return false;
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
        }
    }
}
