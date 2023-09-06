using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API
{
    public enum StatusCodeType
    {
        Informational = 1,  // 1xx 类型的状态码

        Successful = 2,     // 2xx 类型的状态码

        Redirection = 3,    // 3xx 类型的状态码

        ClientError = 4,    // 4xx 类型的状态码

        ServerError = 5     // 5xx 类型的状态码
    }
}
