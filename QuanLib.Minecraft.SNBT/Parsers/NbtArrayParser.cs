using QuanLib.Minecraft.SNBT.Tags;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLib.Minecraft.SNBT.Parsers
{
    public class NbtArrayParser : StringParser<NbtArray>
    {
        private NbtTagParser NbtTagParser { get; }

        public NbtArrayParser(NbtTagParser nbtTagParser = null)
        {
            NbtTagParser = nbtTagParser ?? new NbtTagParser();
        }

        public override NbtArray Parse(string nbt, ref int pos)
        {
            //Before array
            SkipWhitespace(nbt, ref pos);
            if (pos == nbt.Length || nbt[pos] != '[') throw new ArgumentException($"Expected start of array '[' at {pos}: {nbt}");
            pos++;

            //First character of array.
            SkipWhitespace(nbt, ref pos);
            if (pos == nbt.Length) throw new ArgumentException("Array never ends.");

            if (nbt[pos] == ']')
            {
                pos++;
                return new NbtArray();
            }

            //Check ahead if semicolon is found and pick correct array type.
            if (pos + 1 < nbt.Length && nbt[pos + 1] == ';')
            {
                var type = nbt[pos];
                pos += 2;
                if (type == 'B') return CreateArray(NbtArray.ArrayType.Byte, nbt, ref pos);
                else if (type == 'I') return CreateArray(NbtArray.ArrayType.Integer, nbt, ref pos);
                else if (type == 'L') return CreateArray(NbtArray.ArrayType.Long, nbt, ref pos);
                else throw new ArgumentException($"Array of unknown type at {pos}: {nbt}");
            }
            else
            {
                return CreateArray(NbtArray.ArrayType.None, nbt, ref pos);
            }
        }

        /// <summary>
        /// Creates array from nbt, where pos is at the start of the first element (possibly prefixed by whitespace).
        /// </summary>
        /// <param name="type">The type of array to create</param>
        /// <param name="nbt">The nbt to parse</param>
        /// <param name="pos">The pos located after the first item.</param>
        /// <returns>Returns a filled array.</returns>
        private NbtArray CreateArray(NbtArray.ArrayType type, string nbt, ref int pos)
        {
            var result = new NbtArray(type);
            SkipWhitespace(nbt, ref pos);
            if (pos >= nbt.Length) throw new ArgumentException("Array never ends.");
            if (nbt[pos] == ']')
            {
                pos++;
                return result;
            }
            while (true)
            {
                //Parse element and skip whitespace after.
                var value = NbtTagParser.Parse(nbt, ref pos);
                result.Add(value);
                SkipWhitespace(nbt, ref pos);
                if (pos == nbt.Length) throw new ArgumentException("Array never ends.");

                //Process separator character
                if (nbt[pos] == ',')
                {
                    pos++;
                }
                else if (nbt[pos] == ']')
                {
                    pos++;
                    break;
                }
                else throw new ArgumentException($"Invalid character inside array at {pos}: {nbt}");

                //Skip whitespace after separator.
                SkipWhitespace(nbt, ref pos);
                if (pos == nbt.Length) throw new ArgumentException("Array never ends.");
            }
            return result;
        }
    }
}
