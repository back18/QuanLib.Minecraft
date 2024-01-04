using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public static class ManifestParser
    {
        public static Dictionary<string, string> Parse(string text)
        {
            ArgumentNullException.ThrowIfNull(text, nameof(text));

            Dictionary<string, string> dictionary = new();
            if (string.IsNullOrEmpty(text))
                return dictionary;

            string[] lines = text.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (string line in lines)
            {
                string separator = ": ";
                int index = line.IndexOf(separator);
                if (index == -1)
                    continue;

                string key = line[..index];
                string value = line[(index + 1)..];
                dictionary.TryAdd(key, value);
            }

            return dictionary;
        }
    }
}
