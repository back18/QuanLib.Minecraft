using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API.Packet
{
    public class ResponsePacket : DataPacket
    {
        public ResponsePacket(StatusCode statusCode, string type, byte[] data, int id) : base(type, data, id)
        {
            StatusCode = statusCode;
        }

        public ResponsePacket(ResponseModel model) : base(model)
        {
            StatusCode = model.StatusCode!.Value;
        }

        public StatusCode StatusCode { get; }

        public ResponseModel ToModel()
        {
            return new()
            {
                StatusCode = StatusCode,
                Type = Type,
                Data = Data,
                ID = ID,
            };
        }

        public override byte[] Serialize()
        {
            return Serialize(ToModel());
        }

        public static bool TryDeserialize(byte[] bytes, [MaybeNullWhen(false)] out ResponsePacket result)
        {
            if (!TryDeserialize<ResponseModel>(bytes, out var model))
                goto err;

            List<ValidationResult> results = new();
            if (!Validator.TryValidateObject(model, new(model), results, true))
                goto err;

            result = new(model.StatusCode!.Value, model.Type, model.Data, model.ID!.Value);
            return true;

            err:
            result = null;
            return false;
        }

        public class ResponseModel : ModelBase
        {
            [Required(ErrorMessage = "StatusCode参数缺失")]
            public StatusCode? StatusCode { get; set; }
        }
    }
}
