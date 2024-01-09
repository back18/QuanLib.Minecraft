using System.Text.RegularExpressions;

namespace QuanLib.Minecraft.SNBT.Parsers
{
    public abstract class StringParser<T>
    {
        public abstract T Parse(string str, ref int pos);
        public T Parse(string str)
        {
            int pos = 0;
            return Parse(str, ref pos);
        }
        public void SkipWhitespace(string str, ref int pos)
        {
            for (; pos < str.Length; pos++)
            {
                if (char.IsWhiteSpace(str[pos])) continue;
                else break;
            }
        }
    }
}
