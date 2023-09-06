using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API
{
    public class ApiServerException : ApiException
    {
        public ApiServerException(StatusCode errorStatusCode) : base(errorStatusCode, DefaultMessage) { }

        public ApiServerException(StatusCode errorStatusCode, string? message) : base(errorStatusCode, message) { }

        public ApiServerException(StatusCode errorStatusCode, string? sourceErrorType, string? sourceErrorMessage) : base(errorStatusCode, sourceErrorType, sourceErrorMessage, DefaultMessage) { }

        public ApiServerException(StatusCode errorStatusCode, string? sourceErrorType, string? sourceErrorMessage, string? message) : base(errorStatusCode, sourceErrorType, sourceErrorMessage, message) { }

        public ApiServerException(StatusCode errorStatusCode, string? message, Exception? innerException) : base(errorStatusCode, message, innerException) { }

        protected new const string DefaultMessage = "API服务端异常";
    }
}
