using QuanLib.Minecraft.Snbt.Tags;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLib.Minecraft.Snbt.Parsers
{
    public class NbtCompoundParser : StringParser<NbtCompound>
    {
        private NbtTagParser NbtTagParser { get; }
        private NbtStringParser NbtStringParser { get; } = new NbtStringParser();

        public NbtCompoundParser(NbtTagParser nbtTagParser = null)
        {
            NbtTagParser = nbtTagParser ?? new NbtTagParser();
        }

        public override NbtCompound Parse(string nbt, ref int pos)
        {
            //Before compound
            SkipWhitespace(nbt, ref pos);
            if (pos == nbt.Length || nbt[pos] != '{') throw new ArgumentException($"Expected start of compound '{{' at {pos}: {nbt}");
            pos++;

            //First character
            SkipWhitespace(nbt, ref pos);
            if (pos == nbt.Length) throw new ArgumentException("Compound never ends.");
            if (nbt[pos] == '}')
            {
                pos++;
                return new NbtCompound();
            }

            var result = new NbtCompound();
            while (true)
            {
                //Process key
                var key = NbtStringParser.Parse(nbt, ref pos).Result;
                SkipWhitespace(nbt, ref pos);
                if (pos == nbt.Length) throw new ArgumentException("Compound never ends.");

                //Process colon
                if (nbt[pos] != ':') throw new ArgumentException($"Expected colon at {pos}: {nbt}");
                pos++;
                SkipWhitespace(nbt, ref pos);
                if (pos == nbt.Length) throw new ArgumentException("Compound never ends.");

                //Process value
                var value = NbtTagParser.Parse(nbt, ref pos);
                result.Add(key, value);
                SkipWhitespace(nbt, ref pos);
                if (pos == nbt.Length) throw new ArgumentException("Compound never ends.");

                //Process separator character
                if (nbt[pos] == ',')
                {
                    pos++;
                    SkipWhitespace(nbt, ref pos);
                    if (pos == nbt.Length) throw new ArgumentException("Compound never ends.");
                }
                else if (nbt[pos] == '}')
                {
                    pos++;
                    break;
                }
                else throw new ArgumentException($"Invalid character inside compound at {pos}: {nbt}");
            }

            return result;
        }
    }
}
