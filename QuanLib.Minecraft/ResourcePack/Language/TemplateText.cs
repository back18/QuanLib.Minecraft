using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.ResourcePack.Language
{
    public class TemplateText
    {
        private TemplateText(string key, string pattern, string format, int[] orders)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException($"“{nameof(key)}”不能为 null 或空。", nameof(key));
            if (string.IsNullOrEmpty(pattern))
                throw new ArgumentException($"“{nameof(pattern)}”不能为 null 或空。", nameof(pattern));
            if (string.IsNullOrEmpty(format))
                throw new ArgumentException($"“{nameof(format)}”不能为 null 或空。", nameof(format));
            if (orders is null)
                throw new ArgumentNullException(nameof(orders));

            Key = key;
            PatternText = pattern;
            FormatText = format;
            _orders = orders;
        }

        private readonly int[] _orders;

        public string Key { get; }

        public string PatternText { get; }

        public string FormatText { get; }

        public int ArgumentCount => _orders.Length;

        public string Format(params object[] args)
        {
            return string.Format(FormatText, args);
        }

        public string[] Pattern(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException($"“{nameof(input)}”不能为 null 或空。", nameof(input));

            List<string> list = new();
            Match match = Regex.Match(input, PatternText);
            if (match.Success)
                list.AddRange(from Match match1 in match.Groups select match1.Value);

            string[] array = new string[list.Count];
            for (int i = 0; i < array.Length; i++)
                array[i] = list[_orders[i]];

            return array;
        }

        public static bool TryParseLanguage(string key, string value, [MaybeNullWhen(false)] out TemplateText result)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                goto err;

            bool @explicit = false;
            List<string> placeholders = new();
            string pattern = Regex.Replace(value, @"%(?:(\d+)\$)?([A-Za-z%]|$)", match =>
            {
                if (match.Value != "%s")
                    @explicit = true;

                placeholders.Add(match.Value);
                return "(.+)";
            });

            int[] orders = new int[placeholders.Count];
            if (@explicit)
            {
                for (int i = 0; i < orders.Length; i++)
                {
                    Match match = Regex.Match(placeholders[i], @"\d+");
                    if (!match.Success)
                        goto err;
                    if (!int.TryParse(match.Value, out var number))
                        goto err;

                    orders[i] = number - 1;
                }
            }
            else
            {
                for (int i = 0; i < orders.Length; i++)
                    orders[i] = i;
            }

            int index = 0;
            string format = Regex.Replace(pattern, @"\(.+?\)", match => $"{{{orders[index++]}}}");

            result = new(key, pattern, format, orders);
            return true;

            err:
            result = null;
            return false;
        }
    }
}
