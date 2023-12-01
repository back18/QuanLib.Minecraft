using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Snbt
{
    public static class SnbtUtil
    {
        public static string? GlobalMatchStringValue(string sbnt, string key)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(sbnt, nameof(sbnt));
            ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));

            string pattern = key + @": ""([^""]+)""";
            Match match = Regex.Match(sbnt, pattern);
            if (match.Success)
                return match.Groups[1].Value;
            return null;
        }
    }
}
