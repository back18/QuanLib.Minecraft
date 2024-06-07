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
            ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

            Key = key;
            NeedResponse = needResponse;
        }

        public RequestPacket(RequestModel model) : base(model)
        {
            Key = model.Key;
            NeedResponse = model.NeedResponse;
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

            List<ValidationResult> results = [];
            if (!Validator.TryValidateObject(model, new(model), results, true))
                goto err;

            result = new(model.Key!, model.Type, model.Data, model.ID, model.NeedResponse);
            return true;

            err:
            result = null;
            return false;
        }

        public class RequestModel : ModelBase
        {
            [Required(ErrorMessage = "Key参数缺失")]
            public required string Key { get; set; }

            [Required(ErrorMessage = "NeedResponse参数缺失")]
            public required bool NeedResponse { get; set; }
        }
    }
}
