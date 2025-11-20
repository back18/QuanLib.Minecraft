using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Instance.Extensions
{
    public static class LoggerExtension
    {
        public static Microsoft.Extensions.Logging.ILogger AsMicrosoftLogger(this Core.ILogger logger)
        {
            return new MicrosoftLoggerAdapter(logger);
        }
    }
}
