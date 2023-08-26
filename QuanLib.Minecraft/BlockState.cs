using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public class BlockState
    {
        public BlockState(string blockID, IReadOnlyDictionary<string, string> states)
        {
            BlockID = blockID;
            States = states;
        }

        public string BlockID { get; }

        public IReadOnlyDictionary<string, string> States { get; }

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
                string blockID = s[..s.IndexOf('[')];

                result = new(blockID, states);
                return true;
            }
            else
            {
                Dictionary<string, string> states = new();
                string blockID = s;

                result = new(blockID, states);
                return true;
            }
        }

        public override string ToString()
        {
            string s = BlockID;
            if (States.Count > 0)
            {
                s += "[";
                foreach (var state in States)
                {
                    s += $"{state.Key}={state.Value}";
                }
                s += "]";
            }
            return s;
        }
    }
}
