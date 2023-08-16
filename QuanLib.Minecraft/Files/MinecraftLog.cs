using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Files
{
    public class MinecraftLog
    {
        public MinecraftLog(string log)
        {
            if (log is null)
                throw new ArgumentNullException(nameof(log));

            string separator = ": ";
            int index = log.IndexOf(separator);

            if (index == -1)
            {
                Info = string.Empty;
                Message = log;
            }
            else
            {
                Info = log[..index];
                Message = log[(index + separator.Length)..];
            }

            if (Info.Contains("DEBUG"))
                Level = Level.DEBUG;
            else if (Info.Contains("INFO"))
                Level = Level.INFO;
            else if (Info.Contains("WARN"))
                Level = Level.WARN;
            else if (Info.Contains("ERROR"))
                Level = Level.ERROR;
            else if (Info.Contains("FATAL"))
                Level = Level.FATAL;
            else
                Level = Level.DEBUG;
        }

        /// <summary>
        /// 级别
        /// </summary>
        public Level Level { get; }

        /// <summary>
        /// 日志信息
        /// </summary>
        public string Info { get; }

        /// <summary>
        /// 消息/正文
        /// </summary>
        public string Message { get; }

        public override string ToString()
        {
            return $"{Info}: {Message}";
        }
    }
}
