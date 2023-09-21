using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API
{
    public class McapiException : Exception
    {
        public McapiException(StatusCode errorStatusCode) : base(DefaultMessage)
        {
            ErrorStatusCode = errorStatusCode;
        }

        public McapiException(StatusCode errorStatusCode, string? message) : base(message)
        {
            ErrorStatusCode = errorStatusCode;
        }

        public McapiException(StatusCode errorStatusCode, string? message, Exception? innerException) : base(message, innerException)
        {
            ErrorStatusCode = errorStatusCode;
        }

        public McapiException(StatusCode errorStatusCode, string? sourceErrorType, string? sourceErrorMessage, string? message) : base(message)
        {
            ErrorStatusCode = errorStatusCode;
            SourceErrorType = sourceErrorType;
            SourceErrorMessage = sourceErrorMessage;
        }

        protected const string DefaultMessage = "API异常";

        public StatusCode ErrorStatusCode { get; }

        public string? SourceErrorType { get; }

        public string? SourceErrorMessage { get; }

        public override string Message
        {
            get
            {
                if (SourceErrorType is null && SourceErrorMessage is null)
                    return base.Message;
                else
                    return $"{base.Message}\nErrorStatusCode={ErrorStatusCode}({(int)ErrorStatusCode})\nSourceErrorType: {SourceErrorType}\nSourceErrorMessage: {SourceErrorMessage}";
            }
        }
    }
}
