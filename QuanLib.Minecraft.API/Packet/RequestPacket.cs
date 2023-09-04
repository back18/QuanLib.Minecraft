using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API.Packet
{
    public class RequestPacket : DataPacket
    {
        public RequestPacket(string key, string type, byte[] data, int id, bool needResponse) : base(type, data, id)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException($"“{nameof(key)}”不能为 null 或空。", nameof(key));

            Key = key;
            NeedResponse = needResponse;
        }

        public RequestPacket(RequestModel model) : base(model)
        {
            Key = model.Key;
            NeedResponse = model.NeedResponse!.Value;
        }

        public string Key { get; }

        public bool NeedResponse { get; }

        public RequestModel ToModel()
        {
            return new()
            {
                Key = Key,
                Type = Type,
                Data = Data,
                ID = ID,
                NeedResponse = NeedResponse
            };
        }

        public override byte[] Serialize()
        {
            return Serialize(ToModel());
        }

        public static bool TryDeserialize(byte[] bytes, [MaybeNullWhen(false)] out RequestPacket result)
        {
            if (!TryDeserialize<RequestModel>(bytes, out var model))
                goto err;

            List<ValidationResult> results = new();
            if (!Validator.TryValidateObject(model, new(model), results, true))
                goto err;

            result = new(model.Key, model.Type, model.Data, model.ID!.Value, model.NeedResponse!.Value);
            return true;

            err:
            result = null;
            return false;
        }

        public class RequestModel : ModelBase
        {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

            [Required(ErrorMessage = "Key参数缺失")]
            public string Key { get; set; }

            [Required(ErrorMessage = "NeedResponse参数缺失")]
            public bool? NeedResponse { get; set; }
        }
    }
}
