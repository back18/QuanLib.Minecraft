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
        static MinecraftLog()
        {
            _map = new()
            {
                { "DEBUG", Level.DEBUG },
                { "INFO", Level.INFO },
                { "WARN", Level.WARN },
                { "ERROR", Level.ERROR },
                { "FATAL", Level.FATAL }
            };
        }

        public MinecraftLog(DateTime dateTime, string thread, Level level, string type, string message)
        {
            DateTime = dateTime;
            Thread = thread;
            Level = level;
            Type = type;
            Message = message;
        }

        private static readonly Dictionary<string, Level> _map;

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime DateTime { get; }

        /// <summary>
        /// 线程
        /// </summary>
        public string Thread { get; }

        /// <summary>
        /// 级别
        /// </summary>
        public Level Level { get; }

        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// 消息/正文
        /// </summary>
        public string Message { get; }

        public static bool TryParse(string? s, [MaybeNullWhen(false)] out MinecraftLog result)
        {
            if (s is null)
                goto err;

            string separator = ": ";
            int index = s.IndexOf(separator);
            string items, message;

            if (index == -1)
            {
                items = s;
                message = string.Empty;
            }
            else
            {
                items = s[..index];
                message = s[(index + separator.Length)..];
            }

            string pattern = @"\[(.*?)\]";
            MatchCollection matches = Regex.Matches(items, pattern);

            if (matches.Count < 2)
                goto err;

            _ = DateTime.TryParse(matches[0].Groups[1].Value, out var dateTime);

            string[] item2 = matches[1].Groups[1].Value.Split('/');
            if (item2.Length != 2)
                goto err;

            string thread = item2[0];

            if (!_map.TryGetValue(item2[1], out var level))
                goto err;

            string type;
            if (matches.Count > 2)
                type = matches[2].Groups[1].Value;
            else
                type = string.Empty;

            result = new(dateTime, thread, level, type, message);
            return true;

            err:
            result = null;
            return false;
        }

        public override string ToString()
        {
            return $"[{DateTime:hh:mm:ss.fff}] [{Thread}/{Level}] [{Type}]: {Message}";
        }
    }
}
