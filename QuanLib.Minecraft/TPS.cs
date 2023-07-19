using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public class TPS
    {
        public TPS(string dimension, double mspt, double tps)
        {
            Dimension = dimension;
            Mspt = mspt;
            Tps = tps;
        }

        public string Dimension { get; }

        public double Mspt { get; }

        public double Tps { get; }

        public static bool TryParse(string s, [MaybeNullWhen(false)] out TPS result)
        {
            if (s is null)
                throw new ArgumentNullException(nameof(s));

            string pattern1 = @"Mean tick time: (\d+\.\d+) ms\. Mean TPS: (\d+\.\d+)";
            Match match1 = Regex.Match(s, pattern1);
            if (!match1.Success || match1.Groups.Count != 3)
            {
                result = null;
                return false;
            }

            string mspt = match1.Groups[1].Value;
            string tps = match1.Groups[2].Value;
            if (!double.TryParse(mspt, out var mspt2) || !double.TryParse(tps, out var tps2))
            {
                result = null;
                return false;
            }

            string dimension;
            if (s.StartsWith("Overall"))
            {
                dimension = "Overall";
            }
            else
            {
                string pattern2 = @"\((.*?)\)";
                Match match2 = Regex.Match(s, pattern2);
                if (!match2.Success)
                {
                    result = null;
                    return false;
                }
                dimension = match2.Groups[1].Value;
            }

            result = new(dimension, mspt2, tps2);
            return true;
        }

        public static TPS Parse(string s)
        {
            if (TryParse(s, out var result))
                return result;
            else
                throw new FormatException();
        }
    }
}
