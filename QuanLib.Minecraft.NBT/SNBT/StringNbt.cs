using SharpNBT;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace QuanLib.Minecraft.NBT.SNBT
{
    /// <summary>
    /// Provides static methods for parsing string-NBT (SNBT) source text into a complete <see cref="CompoundTag"/>.
    /// </summary>
    public static class StringNbt
    {
        /// <summary>
        /// Parse the given <paramref name="source"/> text into a <see cref="CompoundTag"/>.
        /// </summary>
        /// <param name="source">A string containing the SNBT code to parse.</param>
        /// <returns>The <see cref="CompoundTag"/> instance described in the source text.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="SyntaxErrorException">When <paramref name="source"/> is invalid SNBT code.</exception>
        public static CompoundTag Parse(string source)
        {
            var bytes = Encoding.UTF8.GetBytes(source);
            return Parse(bytes, Encoding.UTF8);
        }

        /// <summary>
        /// Parse the given <paramref name="source"/> text into a <see cref="ListTag"/>.
        /// </summary>
        /// <param name="source">A string containing the SNBT code to parse.</param>
        /// <returns>The <see cref="CompoundTag"/> instance described in the source text.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="SyntaxErrorException">When <paramref name="source"/> is invalid SNBT code.</exception>
        public static ListTag ParseList(string source)
        {
            var bytes = Encoding.UTF8.GetBytes(source);
            return ParseList(bytes, Encoding.UTF8);
        }

        /// <summary>
        /// Parse the given <paramref name="source"/> text into a <see cref="CompoundTag"/>.
        /// </summary>
        /// <param name="source">A string containing the SNBT code to parse.</param>
        /// <param name="encoding">Encoding of the <paramref name="source"/>.</param>
        /// <returns>The <see cref="CompoundTag"/> instance described in the source text.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="SyntaxErrorException">When <paramref name="source"/> is invalid SNBT code.</exception>
        public static CompoundTag Parse(ReadOnlySpan<byte> source, Encoding? encoding = null)
        {
            var scanner = new Scanner(source, encoding ?? Encoding.UTF8);
            scanner.MoveNext(true, true);
            scanner.AssertChar('{');
            return ParseCompound(null, ref scanner);
        }

        /// <summary>
        /// Parse the given <paramref name="source"/> text into a <see cref="ListTag"/>.
        /// </summary>
        /// <param name="source">A string containing the SNBT code to parse.</param>
        /// <param name="encoding">Encoding of the <paramref name="source"/>.</param>
        /// <returns>The <see cref="CompoundTag"/> instance described in the source text.</returns>
        /// <exception cref="ArgumentNullException">When <paramref name="source"/> is <see langword="null"/>.</exception>
        /// <exception cref="SyntaxErrorException">When <paramref name="source"/> is invalid SNBT code.</exception>
        public static ListTag ParseList(ReadOnlySpan<byte> source, Encoding? encoding = null)
        {
            var scanner = new Scanner(source, encoding ?? Encoding.UTF8);
            scanner.MoveNext(true, true);
            scanner.AssertChar('[');
            return ParseList(null, ref scanner);
        }

        private static CompoundTag ParseCompound(string? name, ref Scanner scanner)
        {
            scanner.MoveNext(true, true);

            // For the case of "{}", return empty compound tag. 
            var result = new CompoundTag(name);
            if (scanner.Current == '}')
                return result;

            while (!scanner.IsEndOfInput)
            {
                // Read the name of the tag
                var childName = ParseString(ref scanner, out _);

                // Move to and asser the next significant character is a deliminator.
                scanner.MoveNext(true, true);
                scanner.AssertChar(':');

                // Move to and parse the tag value
                scanner.MoveNext(true, true);
                var tag = ParseTag(childName, ref scanner);
                result.Add(tag);
                scanner.MoveNext(true, true);

                // Comma encountered, read another tag.
                if (scanner.Current == ',')
                {
                    scanner.MoveNext(true, true);
                    continue;
                }

                // Closing brace encountered, break loop.
                if (scanner.Current == '}')
                {
                    // scanner.MoveNext(true, false);
                    break;
                }

                // Invalid character
                scanner.SyntaxError($"Expected ',' or '}}', got '{scanner.Current}'.");
            }

            return result;
        }

        /// <summary>
        /// Parses the next logical chunk of data as a string.
        /// </summary>
        /// <param name="scanner">A reference to the <see cref="Scanner"/> context.</param>
        /// <param name="quoted">
        /// Flag indicating if the value was enclosed in a pair of matching single/double quotes, otherwise
        /// <see langword="false"/> if it was read as a literal span of characters from the input.
        /// </param>
        /// <returns>The scanned string.</returns>
        private static string ParseString(ref Scanner scanner, out bool quoted)
        {
            var quote = scanner.Current;
            if (quote != '"' && quote != '\'')
            {
                quoted = false;
                return ParseUnquotedString(ref scanner);
            }

            quoted = true;
            var escape = false;
            var closed = false;
            var sb = new StringBuilder();

            while (scanner.MoveNext(false, false))
            {
                if (escape)
                {
                    escape = false;
                    sb.Append(scanner.Current);
                    continue;
                }

                if (scanner.Current == quote)
                {
                    closed = true;
                    break;
                }

                if (scanner.Current == '\\')
                {
                    // TODO: Control characters like \r \n, \t, etc.
                    escape = true;
                    continue;
                }

                sb.Append(scanner.Current);
            }

            if (!closed)
                scanner.SyntaxError("Improperly terminated string.");
            return sb.ToString();
        }

        private static string ParseUnquotedString(ref Scanner scanner)
        {
            var start = scanner.Position;
            for (var length = 0; scanner.MoveNext(false, true); length++)
            {
                if (scanner.Current.IsValidUnquoted())
                    continue;

                // Step back one character so not to consume the one that failed the permission check.
                scanner.Position--;
                return new string(scanner.Source.Slice(start, length + 1));
            }

            return string.Empty;
        }

        private static Tag ParseTag(string? name, ref Scanner scanner)
        {
            return scanner.Current switch
            {
                '{' => ParseCompound(name, ref scanner),
                '[' => ParseArray(name, ref scanner),
                _ => ParseLiteral(name, ref scanner)
            };
        }

        private static Tag ParseLiteral(string? name, ref Scanner scanner)
        {
            // Read the input as a string
            var value = ParseString(ref scanner, out var quoted);
            if (quoted || value.Length == 0)
                return new StringTag(name, value);

            // Early out for true/false values
            if (bool.TryParse(value, out var boolean))
                return new ByteTag(name, boolean);

            var suffix = value[^1];
            if (char.IsNumber(suffix))
            {
                // int and double do not require a suffix
                if (value.Contains('.') && double.TryParse(value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var f64))
                    return new DoubleTag(name, f64);

                if (int.TryParse(value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var i32))
                    return new IntTag(name, i32);
            }
            else if (TryParseNumber(name, value, suffix, out var tag))
            {
                return tag;
            }

            if (value.Length > 2 && value[0] == '0' && char.ToLowerInvariant(value[1]) == 'x')
            {
                // TODO: The "official" spec doesn't seem to support hexadecimal numbers
                if (int.TryParse(value[2..], NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out var hex))
                    return new IntTag(name, hex);
            }

            // When all else fails, is could only have been an unquoted string
            return new StringTag(name, value);
        }

        private static bool TryParseNumber(string? name, string value, char suffix, out Tag tag)
        {
            // A much less complicated char.ToLower()
            if (suffix >= 'a')
                suffix -= (char)32;

            switch (suffix)
            {
                case 'B':
                    if (int.TryParse(value[..^1], NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var u8))
                    {
                        tag = new ByteTag(name, u8);
                        return true;
                    }
                    break;
                case 'S':
                    if (short.TryParse(value[..^1], NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var i16))
                    {
                        tag = new ShortTag(name, i16);
                        return true;
                    }
                    break;
                case 'L':
                    if (long.TryParse(value[..^1], NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var i64))
                    {
                        tag = new LongTag(name, i64);
                        return true;
                    }
                    break;
                case 'F':
                    if (float.TryParse(value[..^1], NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var f32))
                    {
                        tag = new FloatTag(name, f32);
                        return true;
                    }
                    break;
                case 'D':
                    if (double.TryParse(value[..^1], NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var f64))
                    {
                        tag = new DoubleTag(name, f64);
                        return true;
                    }
                    break;
            }

            tag = null!;
            return false;
        }

        private static Tag ParseArray(string? name, ref Scanner scanner)
        {
            scanner.MoveNext(true, true);
            if (scanner.Current == ']')
                return new ListTag(name, TagType.End);

            // No type-prefix, must be a ListTag
            if (scanner.Peek() != ';')
                return ParseList(name, ref scanner);

            // This is an array of integral values
            var prefix = scanner.Current;
            scanner.Position += 2;
            return prefix switch
            {
                'B' => new ByteArrayTag(name, ParseArrayValues<byte>(ref scanner)),
                'I' => new IntArrayTag(name, ParseArrayValues<int>(ref scanner)),
                'L' => new LongArrayTag(name, ParseArrayValues<long>(ref scanner)),
                _ => throw scanner.SyntaxError($"Invalid type specifier. Expected 'B', 'I', or 'L', got '{prefix}'.")
            };
        }

        private static ListTag ParseList(string? name, ref Scanner scanner)
        {
            var list = new List<Tag>();
            while (true)
            {
                var child = ParseTag(null, ref scanner);
                list.Add(child);

                scanner.MoveNext(true, true);

                // Comma encountered, read another tag.
                if (scanner.Current == ',')
                {
                    scanner.MoveNext(true, true);
                    continue;
                }

                // Closing brace encountered, break loop.
                if (scanner.Current == ']')
                {
                    break;
                }

                // Invalid character
                scanner.SyntaxError($"Expected ',' or ']', got '{scanner.Current}'.");
            }

            var childType = list.Count > 0 ? list[0].Type : TagType.End;
            return new ListTag(name, childType, list);
        }

        private static T[] ParseArrayValues<T>(ref Scanner scanner) where T : INumber<T>, IParsable<T>
        {
            // Early-out for []
            if (scanner.Current == ']')
            {
                return Array.Empty<T>();
            }

            var start = scanner.Position;
            while (scanner.MoveNext(true, true))
            {
                var c = char.ToLowerInvariant(scanner.Current);
                if (c == ']')
                    break;
                if (char.IsNumber(c) || c == '-' || c == ',')
                    continue;
                if (c is not ('b' or 'l'))
                    scanner.SyntaxError($"Invalid character '{c}' in integer array.");
            }

            var span = scanner.Source.Slice(start, scanner.Position - start);
            var strings = new string(span).Split(SplitSeparators, SplitOpts);

            var values = new T[strings.Length];
            for (var i = 0; i < values.Length; i++)
                values[i] = T.Parse(strings[i], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);

            return values;
        }

        private const StringSplitOptions SplitOpts = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
        private static readonly char[] SplitSeparators = new[] { ',', 'b', 'B', 'l', 'L' };
    }
}
