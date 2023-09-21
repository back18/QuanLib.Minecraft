using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API
{
    public class McapiClientException : McapiException
    {
        public McapiClientException(StatusCode errorStatusCode) : base(errorStatusCode, DefaultMessage) { }

        public McapiClientException(StatusCode errorStatusCode, string? message) : base(errorStatusCode, message) { }

        public McapiClientException(StatusCode errorStatusCode, string? sourceErrorType, string? sourceErrorMessage) : base(errorStatusCode, sourceErrorType, sourceErrorMessage, DefaultMessage) { }

        public McapiClientException(StatusCode errorStatusCode, string? sourceErrorType, string? sourceErrorMessage, string? message) : base(errorStatusCode, sourceErrorType, sourceErrorMessage, message) { }

        public McapiClientException(StatusCode errorStatusCode, string? message, Exception? innerException) : base(errorStatusCode, message, innerException) { }

        protected new const string DefaultMessage = "API客户端异常";
    }
}
