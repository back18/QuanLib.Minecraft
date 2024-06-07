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
            StatusCode = (StatusCode)model.StatusCode;
        }

        public StatusCode StatusCode { get; }

        public ResponseModel ToModel()
        {
            return new()
            {
                StatusCode = (int)StatusCode,
                Type = Type,
                Data = Data,
                ID = ID,
            };
        }

        public void ValidateStatusCode()
        {
            StatusCodeType type = StatusCode.TryeOf();
            ErrorResponseData data;
            switch (type)
            {
                case StatusCodeType.ClientError:
                    data = Data.DeserializeBson<ErrorResponseData>();
                    throw new McapiClientException(StatusCode, data.ErrorType, data.ErrorMessage);
                case StatusCodeType.ServerError:
                    data = Data.DeserializeBson<ErrorResponseData>();
                    throw new McapiServerException(StatusCode, data.ErrorType, data.ErrorMessage);
            }
        }

        public override byte[] Serialize()
        {
            return Serialize(ToModel());
        }

        public static bool TryDeserialize(byte[] bytes, [MaybeNullWhen(false)] out ResponsePacket result)
        {
            if (!TryDeserialize<ResponseModel>(bytes, out var model))
                goto err;

            List<ValidationResult> results = [];
            if (!Validator.TryValidateObject(model, new(model), results, true))
                goto err;

            result = new((StatusCode)model.StatusCode, model.Type, model.Data, model.ID);
            return true;

            err:
            result = null;
            return false;
        }

        public class ResponseModel : ModelBase
        {
            [Required(ErrorMessage = "StatusCode参数缺失")]
            public required int StatusCode { get; set; }
        }

        public class ErrorResponseData
        {
            public string? ErrorType { get; set; }

            public string? ErrorMessage { get; set; }
        }
    }
}
