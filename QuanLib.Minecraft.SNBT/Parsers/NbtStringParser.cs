using QuanLib.Minecraft.SNBT.Converters;
using QuanLib.Minecraft.SNBT.Tags;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace QuanLib.Minecraft.SNBT.Parsers
{
    public class NbtStringParser : StringParser<NbtStringParser.ParseResult>
    {
        private static readonly char[] allowedUnquoted = new char[] {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            '+', '-', '_', '.'
        };

        public struct ParseResult
        {
            public string Result { get; }
            public bool Quoted { get; }
            public ParseResult(string result, bool quoted)
            {
                Result = result;
                Quoted = quoted;
            }
        }

        /// <summary>
        /// Parses a nbt compound key. No whitespace around the value allowed.
        /// </summary>
        /// <param name="nbt">The stringified nbt to parse.</param>
        /// <param name="pos">The starting position.</param>
        /// <returns>Returns the parsed compound key and extra details.</returns>
        public override ParseResult Parse(string nbt, ref int pos)
        {
            char? quote = null;
            bool escape = false;
            bool quoteEnd = false;
            if (nbt[pos] == '"') quote = '"';
            else if (nbt[pos] == '\'') quote = '\'';

            var sb = new StringBuilder();

            if (quote != null) pos++;

            for (; pos < nbt.Length; pos++)
            {
                char c = nbt[pos];
                if (quote != null)
                {
                    if (!escape)
                    {
                        if (c == quote)
                        {
                            pos++;
                            quoteEnd = true;
                            break;
                        }
                        else if (c == '\\') escape = true;
                        else sb.Append(c);
                    }
                    else
                    {
                        escape = false;
                        sb.Append(c);
                    }
                }
                else
                {
                    if (!allowedUnquoted.Contains(c)) break;
                    sb.Append(c);
                }
            }

            if (pos == nbt.Length && quote != null && !quoteEnd) throw new ArgumentException($"Quoted string is not terminated: {nbt}.");

            var raw = sb.ToString();
            if (raw.Length == 0)
            {
                throw new ArgumentException($"Expected primitive nbt value at {pos}: {nbt}");
            }
            return new ParseResult(raw, quote != null);
        }
    }
}
