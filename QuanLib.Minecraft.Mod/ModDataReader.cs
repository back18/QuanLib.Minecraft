using QuanLib.IO.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod
{
    public static class ModDataReader
    {
        public static ModData[] FromLinesRead(string[] lines)
        {
            ArgumentNullException.ThrowIfNull(lines, nameof(lines));

            List<ModData> modDatas = [];
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrEmpty(line) || line.StartsWith('#'))
                    continue;

                try
                {
                    ModData modData = ModData.Parse(line);
                    modDatas.Add(modData);
                }
                catch (FormatException ex)
                {
                    throw new FormatException($"在第{i}行引发了格式异常", ex);
                }
            }

            Dictionary<string, ModData> result = [];
            foreach (ModData modData in modDatas)
            {
                foreach (string modId in modData.ModIds)
                {
                    if (string.IsNullOrEmpty(modId))
                        continue;

                    Debug.WriteLineIf(result.TryGetValue(modId, out var otherModData), $"模组\"{modData}\"和\"{otherModData}\"的ID\'{modId}\'重复");
                    result[modId] = modData;
                }
            }

            return modDatas.ToArray();
        }

        public static ModData[] FromHmclRead(string filePath)
        {
            ArgumentException.ThrowIfNullOrEmpty(filePath, nameof(filePath));

            using ZipPack zipPack = new(filePath);
            ZipItem zipItem = zipPack.GetFile("assets/mod_data.txt");
            string[] lines = zipItem.ReadAllLines(Encoding.UTF8);
            return FromLinesRead(lines);
        }
    }
}
