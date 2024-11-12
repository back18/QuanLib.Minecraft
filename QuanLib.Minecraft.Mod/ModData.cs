using QuanLib.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod
{
    public class ModData : IParsable<ModData>
    {
        public ModData(
            IList<string> modIds,
            string subName,
            string mainName,
            string abbrName,
            string curseForgeIndex,
            string mcmodIndex)
        {
            ArgumentNullException.ThrowIfNull(modIds, nameof(modIds));
            ArgumentNullException.ThrowIfNull(mainName, nameof(mainName));
            ArgumentNullException.ThrowIfNull(subName, nameof(subName));
            ArgumentNullException.ThrowIfNull(abbrName, nameof(abbrName));
            ArgumentNullException.ThrowIfNull(curseForgeIndex, nameof(curseForgeIndex));
            ArgumentNullException.ThrowIfNull(mcmodIndex, nameof(mcmodIndex));

            ModIds = modIds.AsReadOnly();
            MainName = mainName;
            SubName = subName;
            AbbrName = abbrName;
            CurseForgeIndex = curseForgeIndex;
            McmodIndex = mcmodIndex;
        }

        public ReadOnlyCollection<string> ModIds { get; }

        public string MainName { get; }

        public string SubName { get; }

        public string AbbrName { get; }

        public string CurseForgeIndex { get; }

        public string McmodIndex { get; }

        public static ModData Parse(string s) => Parse(s, null);

        public static bool TryParse([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out ModData result) => TryParse(s, null, out result);

        public static ModData Parse(string s, IFormatProvider? provider)
        {
            ArgumentException.ThrowIfNullOrEmpty(s, nameof(s));

            string[] items = s.Split(';');
            if (items.Length != 6)
                throw new FormatException("分号分隔后的文本数量应该为6个");

            return new(items[2].Split(','), items[4], items[3], items[5], items[0], items[1]);
        }

        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out ModData result)
        {
            if (string.IsNullOrEmpty(s))
            {
                result = null;
                return false;
            }

            string[] items = s.Split(';');
            if (items.Length != 6)
            {
                result = null;
                return false;
            }

            result = new(items[2].Split(','), items[4], items[3], items[5], items[0], items[1]);
            return true;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new();

            stringBuilder.AppendFormat("【{0}】", MainName);
            if (!string.IsNullOrEmpty(SubName))
                stringBuilder.AppendFormat("{0}", SubName);
            if (!string.IsNullOrEmpty(AbbrName))
                stringBuilder.AppendFormat(" ({0})", AbbrName);

            return stringBuilder.ToString();
        }
    }
}
