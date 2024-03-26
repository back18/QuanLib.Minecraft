using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public class BlockState
    {
        public BlockState(string blockId, IDictionary<string, string> states)
        {
            BlockId = blockId;
            States = states.AsReadOnly();
        }

        public string BlockId { get; }

        public ReadOnlyDictionary<string, string> States { get; }

        public static bool TryParse(string s, [MaybeNullWhen(false)] out BlockState result)
        {
            if (string.IsNullOrEmpty(s))
            {
                result = null;
                return false;
            }

            Match match = Regex.Match(s, @"\[(.*?)\]");
            if (match.Success)
            {
                if (!MinecraftUtil.TryParseBlockState(match.Groups[1].Value, out var states))
                {
                    result = null;
                    return false;
                }
                string blockId = s[..s.IndexOf('[')];

                result = new(blockId, states);
                return true;
            }
            else
            {
                Dictionary<string, string> states = new();
                string blockId = s;

                result = new(blockId, states);
                return true;
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new(BlockId);

            if (States.Count > 0)
            {
                stringBuilder.Append('[');
                foreach (var state in States)
                {
                    stringBuilder.Append(state.Key);
                    stringBuilder.Append('=');
                    stringBuilder.Append(state.Value);
                }
                stringBuilder.Append(']');
            }

            return stringBuilder.ToString();
        }
    }
}
