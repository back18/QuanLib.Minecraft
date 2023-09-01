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
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API.DataPackets
{
    public class DataPacket
    {
        public DataPacket(string key, string type, byte[] data, int id, bool needResponse)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException($"“{nameof(key)}”不能为 null 或空。", nameof(key));
            if (string.IsNullOrEmpty(type))
                throw new ArgumentException($"“{nameof(type)}”不能为 null 或空。", nameof(type));
            if (data is null)
                throw new ArgumentNullException(nameof(data));

            Key = key;
            Type = type;
            Data = data;
            ID = id;
            NeedResponse = needResponse;
        }

        public DataPacket(Model model)
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

            Key = model.Key;
            Type = model.Type;
            Data = model.Data;
            ID = model.ID!.Value;
            NeedResponse = model.NeedResponse!.Value;
        }

        public string Key { get; }

        public string Type { get; }

        public byte[] Data { get; }

        public int ID { get; }

        public bool NeedResponse { get; }

        public Model ToModel()
        {
            return new()
            {
                ID = ID,
                Key = Key,
                Type = Type,
                Data = Data
            };
        }

        public byte[] ToBytes()
        {
            using MemoryStream stream = new();
            BsonSerializer.Serialize(new BsonBinaryWriter(stream), ToModel());
            byte[] length = BitConverter.GetBytes((int)stream.Length + 4);
            Array.Reverse(length);
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(length, 0, length.Length);
            return stream.ToArray();
        }

        public static bool TryParseBytes(byte[] bytes, [MaybeNullWhen(false)] out DataPacket result)
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

                byte[] buffer2 = new byte[stream.Length - 4];
                stream.Read(buffer2, 0, buffer2.Length);
                Model model = BsonSerializer.Deserialize<Model>(buffer2);
                result = new(model);
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

        public class Model
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

            [Required(ErrorMessage = "Key参数缺失")]
            public string Key { get; set; }

            [Required(ErrorMessage = "Type参数缺失")]
            public string Type { get; set; }

            [Required(ErrorMessage = "Data参数缺失")]
            public byte[] Data { get; set; }

            [Required(ErrorMessage = "ID参数缺失")]
            public int? ID { get; set; }

            [Required(ErrorMessage = "NeedResponse参数缺失")]
            public bool? NeedResponse { get; }
        }
    }
}
