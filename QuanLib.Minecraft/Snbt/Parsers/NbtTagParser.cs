using QuanLib.Minecraft.Snbt.Tags;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLib.Minecraft.Snbt.Parsers
{
    public class NbtTagParser : StringParser<NbtTag>
    {
        private NbtPrimitiveParser PrimitiveParser { get; } = new NbtPrimitiveParser();
        private NbtArrayParser ArrayParser { get; }
        private NbtCompoundParser CompoundParser { get; }

        public NbtTagParser()
        {
            ArrayParser = new NbtArrayParser(this);
            CompoundParser = new NbtCompoundParser(this);
        }

        /// <summary>
        /// Parses any nbt and returns the appropriate tag.
        /// </summary>
        /// <param name="nbt">The stringified nbt.</param>
        /// <param name="pos">The starting position.</param>
        /// <returns>Returns the parsed tag.</returns>
        public override NbtTag Parse(string nbt, ref int pos)
        {
            SkipWhitespace(nbt, ref pos);
            if (pos >= nbt.Length) throw new ArgumentException($"Expected tag at {pos}: {nbt}");
            NbtTag result;
            if (nbt[pos] == '[') result = ArrayParser.Parse(nbt, ref pos);
            else if (nbt[pos] == '{') result = CompoundParser.Parse(nbt, ref pos);
            else result = PrimitiveParser.Parse(nbt, ref pos);
            SkipWhitespace(nbt, ref pos);
            return result;
        }
    }
}
