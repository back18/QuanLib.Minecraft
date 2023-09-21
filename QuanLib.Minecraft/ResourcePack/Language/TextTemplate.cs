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
    public class TextTemplate
    {
        private TextTemplate(string pattern, string format, int[] orders)
        {
            if (string.IsNullOrEmpty(pattern))
                throw new ArgumentException($"“{nameof(pattern)}”不能为 null 或空。", nameof(pattern));
            if (string.IsNullOrEmpty(format))
                throw new ArgumentException($"“{nameof(format)}”不能为 null 或空。", nameof(format));
            if (orders is null)
                throw new ArgumentNullException(nameof(orders));

            PatternText = pattern;
            FormatText = format;
            _orders = orders;
        }

        private readonly int[] _orders;

        public string PatternText { get; }

        public string FormatText { get; }

        public int ArgumentCount => _orders.Length;

        public bool TryFormat(object[] args, [MaybeNullWhen(false)] out string result)
        {
            if (args is null || args.Length != ArgumentCount)
                goto fail;

            if (ArgumentCount == 0)
            {
                result = FormatText;
                return true;
            }

            try
            {
                result = string.Format(FormatText, args);
                return true;
            }
            catch
            {
                goto fail;
            }

            fail:
            result = null;
            return false;
        }

        public bool TryMatch(string input, [MaybeNullWhen(false)] out string[] result)
        {
            if (string.IsNullOrEmpty(input))
                goto fail;

            if (ArgumentCount == 0)
            {
                if (input != PatternText)
                    goto fail;

                result = Array.Empty<string>();
                return true;
            }

            List<string> list = new();
            Match match = Regex.Match(input, PatternText);
            if (!match.Success)
                goto fail;

            for (int i = 1; i < match.Groups.Count; i++)
            {
                Group group = match.Groups[i];
                list.Add(group.Value);
            }

            result = new string[list.Count];
            for (int i = 0; i < result.Length; i++)
                result[i] = list[_orders[i]];
            return true;

            fail:
            result = null;
            return false;
        }

        public override string ToString()
        {
            return FormatText;
        }

        public static TextTemplate Parse(string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentException($"“{nameof(s)}”不能为 null 或空。", nameof(s));

            if (TryParse(s, out var template))
                return template;
            else
                throw new FormatException();
        }

        public static bool TryParse(string s, [MaybeNullWhen(false)] out TextTemplate result)
        {
            if (string.IsNullOrEmpty(s))
                goto err;

            bool @explicit = false;
            List<string> placeholders = new();
            string pattern = Regex.Replace(s, @"%(?:(\d+)\$)?([A-Za-z%]|$)", match =>
            {
                if (match.Value != "%s")
                    @explicit = true;

                placeholders.Add(match.Value);
                return "(.+)";
            });

            int[] orders = new int[placeholders.Count];
            string format;
            if (pattern.Length > 0)
            {
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
                format = Regex.Replace(pattern, "\\(\\.\\+\\)", match => $"{{{orders[index++]}}}");
            }
            else
            {
                format = pattern;
            }

            result = new(pattern, format, orders);
            return true;

            err:
            result = null;
            return false;
        }
    }
}
