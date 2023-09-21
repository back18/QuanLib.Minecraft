using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.API
{
    public static class McapiUtil
    {
        public static StatusCodeType TryeOf(this StatusCode source)
        {
            int code = (int)source;
            return (StatusCodeType)(code / 100);
        }
    }
}
