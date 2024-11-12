using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Mod
{
    public class ModDataFactory
    {
        public ModDataFactory(ModData[] modDatas)
        {
            ArgumentNullException.ThrowIfNull(modDatas, nameof(modDatas));

            _modDatas = modDatas;
            _modIdMap = BuildModIdMap(modDatas);
        }

        private readonly ModData[] _modDatas;

        private readonly Dictionary<string, List<ModData>> _modIdMap;

        public ModData[] GetModDatasFromModId(string modId)
        {
            if (_modIdMap.TryGetValue(modId, out var list))
                return list.ToArray();
            else
                return [];
        }

        private static Dictionary<string, List<ModData>> BuildModIdMap(ModData[] modDatas)
        {
            ArgumentNullException.ThrowIfNull(modDatas, nameof(modDatas));

            Dictionary<string, List<ModData>> modIdMap = [];
            foreach (ModData modData in modDatas)
            {
                foreach (string modId in modData.ModIds)
                {
                    if (string.IsNullOrEmpty(modId))
                        continue;

                    if (modIdMap.TryGetValue(modId, out var list))
                    {
                        list.Add(modData);
                    }
                    else
                    {
                        list = new(1) { modData };
                        modIdMap.Add(modId, list);
                    }
                }
            }
            return modIdMap;
        }
    }
}
