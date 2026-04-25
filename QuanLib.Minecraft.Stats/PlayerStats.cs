using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace QuanLib.Minecraft.Stats
{
    public class PlayerStats
    {
        public const string STATS = "stats";
        public const string CUSTOM = "minecraft:custom";
        public const string ITEM_PICKED_UP = "minecraft:picked_up";
        public const string ITEM_DROPPED = "minecraft:dropped";
        public const string ITEM_USED = "minecraft:used";
        public const string ITEM_CRAFTED = "minecraft:crafted";
        public const string ITEM_BROKEN = "minecraft:broken";
        public const string BLOCK_MINED = "minecraft:mined";
        public const string ENTITY_KILLED = "minecraft:killed";
        public const string ENTITY_KILLED_BY = "minecraft:killed_by";

        public static readonly ReadOnlySet<string> StatsTypes =
        [
            CUSTOM,
            ITEM_PICKED_UP,
            ITEM_DROPPED,
            ITEM_USED,
            ITEM_CRAFTED,
            ITEM_BROKEN,
            BLOCK_MINED,
            ENTITY_KILLED,
            ENTITY_KILLED_BY
        ];

        public static readonly PlayerStats Empty = new()
        {
            CustomStats = CustomStats.Load(ReadOnlyDictionary<string, int>.Empty),
            ItemPickedUp = ReadOnlyDictionary<string, int>.Empty,
            ItemDropped = ReadOnlyDictionary<string, int>.Empty,
            ItemUsed = ReadOnlyDictionary<string, int>.Empty,
            ItemCrafted = ReadOnlyDictionary<string, int>.Empty,
            ItemBroken = ReadOnlyDictionary<string, int>.Empty,
            BlockMined = ReadOnlyDictionary<string, int>.Empty,
            EntityKilled = ReadOnlyDictionary<string, int>.Empty,
            EntityKilledBy = ReadOnlyDictionary<string, int>.Empty,
        };

        /// <summary>
        /// 与玩家行为相关的大量通用统计信息。
        /// </summary>
        public required CustomStats CustomStats { get; init; }

        /// <summary>
        /// 与玩家捡起的掉落物品数量有关的统计信息。
        /// </summary>
        public required ReadOnlyDictionary<string, int> ItemPickedUp { get; init; }

        /// <summary>
        /// 与丢弃的物品数量有关的统计信息。
        /// </summary>
        public required ReadOnlyDictionary<string, int> ItemDropped { get; init; }

        /// <summary>
        /// 与使用的方块或物品的数量有关的统计信息。
        /// </summary>
        public required ReadOnlyDictionary<string, int> ItemUsed { get; init; }

        /// <summary>
        /// 与合成、熔炼等物品数量有关的统计信息。
        /// </summary>
        public required ReadOnlyDictionary<string, int> ItemCrafted { get; init; }

        /// <summary>
        /// 与玩家的物品耐久度相关的统计信息。
        /// </summary>
        public required ReadOnlyDictionary<string, int> ItemBroken { get; init; }

        /// <summary>
        /// 与玩家开采的方块数相关的统计信息。
        /// </summary>
        public required ReadOnlyDictionary<string, int> BlockMined { get; init; }

        /// <summary>
        /// 与玩家杀死的实体数量相关的统计信息。
        /// </summary>
        public required ReadOnlyDictionary<string, int> EntityKilled { get; init; }

        /// <summary>
        /// 与玩家被实体杀死相关的统计信息。
        /// </summary>
        public required ReadOnlyDictionary<string, int> EntityKilledBy { get; init; }

        public IReadOnlyDictionary<string, int> GetStatsFromType(string statsType)
        {
            return statsType switch
            {
                CUSTOM => CustomStats,
                ITEM_PICKED_UP => ItemPickedUp,
                ITEM_DROPPED => ItemDropped,
                ITEM_USED => ItemUsed,
                ITEM_CRAFTED => ItemCrafted,
                ITEM_BROKEN => ItemBroken,
                BLOCK_MINED => BlockMined,
                ENTITY_KILLED => EntityKilled,
                ENTITY_KILLED_BY => EntityKilledBy,
                _ => throw new ArgumentException("Invalid stats type.", nameof(statsType))
            };
        }

        public static PlayerStats Parse(string json)
        {
            ArgumentException.ThrowIfNullOrEmpty(json, nameof(json));

            JObject jobj = JObject.Parse(json);

            if (jobj[STATS] is not JObject stats)
                return Empty;

            IReadOnlyDictionary<string, int> customStats = ParseCustomStats(stats);
            ReadOnlyDictionary<string, int> itemPickedUp = Parse(stats, ITEM_PICKED_UP);
            ReadOnlyDictionary<string, int> itemDropped = Parse(stats, ITEM_DROPPED);
            ReadOnlyDictionary<string, int> itemUsed = Parse(stats, ITEM_USED);
            ReadOnlyDictionary<string, int> itemCrafted = Parse(stats, ITEM_CRAFTED);
            ReadOnlyDictionary<string, int> itemBroken = Parse(stats, ITEM_BROKEN);
            ReadOnlyDictionary<string, int> blockMined = Parse(stats, BLOCK_MINED);
            ReadOnlyDictionary<string, int> entityKilled = Parse(stats, ENTITY_KILLED);
            ReadOnlyDictionary<string, int> entityKilledBy = Parse(stats, ENTITY_KILLED_BY);

            return new PlayerStats()
            {
                CustomStats = CustomStats.Load(customStats),
                ItemPickedUp = itemPickedUp,
                ItemDropped = itemDropped,
                ItemUsed = itemUsed,
                ItemCrafted = itemCrafted,
                ItemBroken = itemBroken,
                BlockMined = blockMined,
                EntityKilled = entityKilled,
                EntityKilledBy = entityKilledBy
            };
        }

        private static ReadOnlyDictionary<string, int> Parse(JObject stats, string key)
        {
            Dictionary<string, int>? result = stats[key]?.ToObject<Dictionary<string, int>>();
            if (result is not null)
                return result.AsReadOnly();

            return ReadOnlyDictionary<string, int>.Empty;
        }

        private static IReadOnlyDictionary<string, int> ParseCustomStats(JObject stats)
        {
            Dictionary<string, int>? result = stats[CUSTOM]?.ToObject<Dictionary<string, int>>();
            if (result is not null)
                return result;

            return ReadOnlyDictionary<string, int>.Empty;
        }
    }
}
