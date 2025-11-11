using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace QuanLib.Minecraft.Logging
{
    public readonly partial struct MinecraftLog(TimeOnly time, Level level, string thread, string message, string? logger = null) : IParsable<MinecraftLog>
    {
        private const string SEPARATOR = ": ";

        public readonly TimeOnly Time = time;

        public readonly Level Level = level;

        public readonly string Thread = thread;

        public readonly string Message = message;

        public readonly string? Logger = logger;

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Logger))
                return $"[{Time:HH:mm:ss}] [{Thread}/{Level}]: {Message}";
            else
                return $"[{Time:HH:mm:ss}] [{Thread}/{Level}] [{Logger}]: {Message}";
        }

        public static MinecraftLog Parse(string s)
            => Parse(s, null);

        public static bool TryParse([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out MinecraftLog result)
            => TryParse(s, null, out result);

        public static MinecraftLog Parse(string s, IFormatProvider? provider)
        {
            if (TryParse(s, provider, out MinecraftLog result))
                return result;
            else
                throw new FormatException();
        }

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out MinecraftLog result)
        {
            if (string.IsNullOrEmpty(s))
            {
                result = new(TimeOnly.MinValue, Level.INFO, string.Empty, string.Empty);
                return true;
            }

            string[] lines = s.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);
            ReadOnlySpan<char> firstLine = lines[0].AsSpan();

            int index = firstLine.IndexOf(SEPARATOR);
            if (index == -1)
            {
                result = new(TimeOnly.MinValue, Level.INFO, string.Empty, s);
                return true;
            }

            ReadOnlySpan<char> left = firstLine[..index];
            ReadOnlySpan<char> right = firstLine[(index + SEPARATOR.Length)..];

            List<(int start, int length)> ranges = [];
            Regex regex = MatchBracketsRegex();

            foreach (ValueMatch match in regex.EnumerateMatches(left))
                ranges.Add((match.Index + 1, match.Length - 2));

            TimeOnly time = TimeOnly.MinValue;
            Level level = Level.INFO;
            string thread = string.Empty;
            string message = string.Empty;
            string? logger = null;

            if (ranges.Count >= 1)
            {
                (int start, int length) range = ranges[0];
                ReadOnlySpan<char> span = left.Slice(range.start, range.length);
                const int length = 8;

                if (span.Length < length)
                    goto failed;

                if (span.Length > length)
                {
                    int index2 = span.IndexOf(':');

                    if (index2 == -1)
                        goto failed;

                    int start = index2 - 2;
                    if (start < 0 || start + length > span.Length || span[index2 + 3] != ':')
                        goto failed;

                    span = span.Slice(start, length);
                }

                if (!TimeOnly.TryParse(span, out time))
                    goto failed;
            }

            if (ranges.Count >= 2)
            {
                (int start, int length) range = ranges[1];
                ReadOnlySpan<char> span = left.Slice(range.start, range.length);
                ReadOnlySpan<char> levelSpan;
                ReadOnlySpan<char> threadSpan;

                int index2 = span.IndexOf('/');
                if (index2 == -1)
                {
                    levelSpan = span;
                    threadSpan = ReadOnlySpan<char>.Empty;
                }
                else
                {
                    levelSpan = span[(index2 + 1)..];
                    threadSpan = span[..index2];
                }

                int levelId = ParseLevel(levelSpan);
                if (levelId == -1)
                    goto failed;

                level = (Level)levelId;
                thread = threadSpan.ToString();
            }

            if (ranges.Count >= 3)
            {
                (int start, int length) range = ranges[2];
                ReadOnlySpan<char> span = left.Slice(range.start, range.length);
                logger = span.ToString();
            }

            if (lines.Length > 1)
            {
                lines[0] = right.ToString();
                message = string.Join(Environment.NewLine, lines);
            }
            else
            {
                message = right.ToString();
            }

            result = new(time, level, thread, message, logger);
            return true;

            failed:
            result = default;
            return false;
        }

        private static int ParseLevel(ReadOnlySpan<char> levelSpan)
        {
            if (levelSpan.Length == 4)
            {
                // INFO
                if ((levelSpan[0] == 'I' || levelSpan[0] == 'i') &&
                    (levelSpan[1] == 'N' || levelSpan[1] == 'n') &&
                    (levelSpan[2] == 'F' || levelSpan[2] == 'f') &&
                    (levelSpan[3] == 'O' || levelSpan[3] == 'o'))
                    return 1;

                // WARN
                if ((levelSpan[0] == 'W' || levelSpan[0] == 'w') &&
                    (levelSpan[1] == 'A' || levelSpan[1] == 'a') &&
                    (levelSpan[2] == 'R' || levelSpan[2] == 'r') &&
                    (levelSpan[3] == 'N' || levelSpan[3] == 'n'))
                    return 2;
            }
            else if (levelSpan.Length == 5)
            {
                // DEBUG
                if ((levelSpan[0] == 'D' || levelSpan[0] == 'd') &&
                    (levelSpan[1] == 'E' || levelSpan[1] == 'e') &&
                    (levelSpan[2] == 'B' || levelSpan[2] == 'b') &&
                    (levelSpan[3] == 'U' || levelSpan[3] == 'u') &&
                    (levelSpan[4] == 'G' || levelSpan[4] == 'g'))
                    return 0;

                // ERROR  
                if ((levelSpan[0] == 'E' || levelSpan[0] == 'e') &&
                    (levelSpan[1] == 'R' || levelSpan[1] == 'r') &&
                    (levelSpan[2] == 'R' || levelSpan[2] == 'r') &&
                    (levelSpan[3] == 'O' || levelSpan[3] == 'o') &&
                    (levelSpan[4] == 'R' || levelSpan[4] == 'r'))
                    return 3;

                // FATAL
                if ((levelSpan[0] == 'F' || levelSpan[0] == 'f') &&
                    (levelSpan[1] == 'A' || levelSpan[1] == 'a') &&
                    (levelSpan[2] == 'T' || levelSpan[2] == 't') &&
                    (levelSpan[3] == 'A' || levelSpan[3] == 'a') &&
                    (levelSpan[4] == 'L' || levelSpan[4] == 'l'))
                    return 4;
            }

            return -1;
        }

        [GeneratedRegex(@"\[(.*?)\]", RegexOptions.Compiled)]
        private static partial Regex MatchBracketsRegex();
    }
}
