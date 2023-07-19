using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinecraftPlayerRanking
{
    /// <summary>
    /// 统计数据
    /// </summary>
    public class Stats
    {
        public Stats(Json json)
        {
            CUSTOM = new ReadOnlyDictionary<string, int>(json.CUSTOM);
            BLOCK_MINED = new ReadOnlyDictionary<string, int>(json.BLOCK_MINED);
            ITEM_BROKEN = new ReadOnlyDictionary<string, int>(json.ITEM_BROKEN);
            ITEM_CRAFTED = new ReadOnlyDictionary<string, int>(json.ITEM_CRAFTED);
            ITEM_USED = new ReadOnlyDictionary<string, int>(json.ITEM_USED);
            ITEM_PICKED_UP = new ReadOnlyDictionary<string, int>(json.ITEM_PICKED_UP);
            ITEM_DROPPED = new ReadOnlyDictionary<string, int>(json.ITEM_DROPPED);
            ENTITY_KILLED = new ReadOnlyDictionary<string, int>(json.ENTITY_KILLED);
            ENTITY_KILLED_BY = new ReadOnlyDictionary<string, int>(json.ENTITY_KILLED_BY);
        }

        public IReadOnlyDictionary<string, int> this[StatsType statsType]
        {
            get => statsType switch
            {
                StatsType.CUSTOM => CUSTOM,
                StatsType.BLOCK_MINED => BLOCK_MINED,
                StatsType.ITEM_BROKEN => ITEM_BROKEN,
                StatsType.ITEM_CRAFTED => ITEM_CRAFTED,
                StatsType.ITEM_USED => ITEM_USED,
                StatsType.ITEM_PICKED_UP => ITEM_PICKED_UP,
                StatsType.ITEM_DROPPED => ITEM_DROPPED,
                StatsType.ENTITY_KILLED => ENTITY_KILLED,
                StatsType.ENTITY_KILLED_BY => ENTITY_KILLED_BY,
                _ => throw new NotImplementedException()
            };
        }

        public IReadOnlyDictionary<string, int> CUSTOM { get; }

        public IReadOnlyDictionary<string, int> BLOCK_MINED { get; }

        public IReadOnlyDictionary<string, int> ITEM_BROKEN { get; }

        public IReadOnlyDictionary<string, int> ITEM_CRAFTED { get; }

        public IReadOnlyDictionary<string, int> ITEM_USED { get; }

        public IReadOnlyDictionary<string, int> ITEM_PICKED_UP { get; }

        public IReadOnlyDictionary<string, int> ITEM_DROPPED { get; }

        public IReadOnlyDictionary<string, int> ENTITY_KILLED { get; }

        public IReadOnlyDictionary<string, int> ENTITY_KILLED_BY { get; }

        public class Json
        {
            public Json(
                Dictionary<string, int> custom,
                Dictionary<string, int> block_mined,
                Dictionary<string, int> item_broken,
                Dictionary<string, int> item_crafted,
                Dictionary<string, int> item_used,
                Dictionary<string, int> item_picked_up,
                Dictionary<string, int> item_dropped,
                Dictionary<string, int> entity_killed,
                Dictionary<string, int> entity_killed_by)
            {
                CUSTOM = custom;
                BLOCK_MINED = block_mined;
                ITEM_BROKEN = item_broken;
                ITEM_CRAFTED = item_crafted;
                ITEM_USED = item_used;
                ITEM_PICKED_UP = item_picked_up;
                ITEM_DROPPED = item_dropped;
                ENTITY_KILLED = entity_killed;
                ENTITY_KILLED_BY = entity_killed_by;
            }

            [JsonProperty("minecraft:custom")]
            public Dictionary<string, int> CUSTOM { get; set; }

            [JsonProperty("minecraft:mined")]
            public Dictionary<string, int> BLOCK_MINED { get; set; }

            [JsonProperty("minecraft:broken")]
            public Dictionary<string, int> ITEM_BROKEN { get; set; }

            [JsonProperty("minecraft:crafted")]
            public Dictionary<string, int> ITEM_CRAFTED { get; set; }

            [JsonProperty("minecraft:used")]
            public Dictionary<string, int> ITEM_USED { get; set; }

            [JsonProperty("minecraft:picked_up")]
            public Dictionary<string, int> ITEM_PICKED_UP { get; set; }

            [JsonProperty("minecraft:dropped")]
            public Dictionary<string, int> ITEM_DROPPED { get; set; }

            [JsonProperty("minecraft:killed")]
            public Dictionary<string, int> ENTITY_KILLED { get; set; }

            [JsonProperty("minecraft:killed_by")]
            public Dictionary<string, int> ENTITY_KILLED_BY { get; set; }
        }
    }
}
