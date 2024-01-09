using QuanLib.Minecraft.SNBT.Converters;
using QuanLib.Minecraft.SNBT.Tags;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace QuanLib.Minecraft.SNBT.Parsers
{
    public class NbtPrimitiveParser : StringParser<NbtPrimitive>
    {
        private static readonly char[] numberTypes = new char[] { 'b', 's', 'l', 'f', 'd' };
        private NbtStringParser StringParser { get; } = new NbtStringParser();

        /// <summary>
        /// Parses primitive stringified nbt. No whitespace around the value allowed.
        /// See also <see cref="NbtPrimitive"/> for the available types.
        /// </summary>
        /// <param name="nbt">The stringified nbt to parse.</param>
        /// <param name="pos">The starting position.</param>
        /// <returns>Returns the parsed primitive</returns>
        public override NbtPrimitive Parse(string nbt, ref int pos)
        {
            var result = StringParser.Parse(nbt, ref pos);
            var value = result.Result;
            if (result.Quoted)
            {
                return new NbtPrimitive<string>(value);
            }
            else if (bool.TryParse(value, out bool boolValue))
            {
                return new NbtPrimitive<bool>(boolValue);
            }
            else if (char.ToLowerInvariant(value[^1]) == 'b' && sbyte.TryParse(value[..^1], NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out var byteValue))
            {
                return new NbtPrimitive<sbyte>(byteValue);
            }
            else if (char.ToLowerInvariant(value[^1]) == 's' && short.TryParse(value[..^1], NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out var shortValue))
            {
                return new NbtPrimitive<short>(shortValue);
            }
            else if (char.ToLowerInvariant(value[^1]) == 'l' && long.TryParse(value[..^1], NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out var longValue))
            {
                return new NbtPrimitive<long>(longValue);
            }
            else if (char.ToLowerInvariant(value[^1]) == 'd' && double.TryParse(value[..^1], NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo, out var doubleValue))
            {
                return new NbtPrimitive<double>(doubleValue);
            }
            else if (char.ToLowerInvariant(value[^1]) == 'f' && float.TryParse(value[..^1], NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo, out var floatValue))
            {
                return new NbtPrimitive<float>(floatValue);
            }
            else if (int.TryParse(value, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo, out var intValue))
            {
                return new NbtPrimitive<int>(intValue);
            }
            else
            {
                return new NbtPrimitive<string>(value);
            }
        }
    }
}
