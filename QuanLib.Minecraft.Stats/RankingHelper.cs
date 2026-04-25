using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLib.Minecraft.Stats
{
    public static class RankingHelper
    {
        public static RankingInfo[] GetCustomStatsRanking(this IReadOnlyDictionary<Guid, PlayerStats> stats, string key)
        {
            ArgumentNullException.ThrowIfNull(stats, nameof(stats));
            ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

            return stats.Select(s => new RawRankingInfo(s.Key, s.Value.CustomStats.GetValueOrDefault(key, 0))).Ranking();
        }

        public static RankingInfo[] GetItemPickedUpRanking(this IReadOnlyDictionary<Guid, PlayerStats> stats)
        {
            ArgumentNullException.ThrowIfNull(stats, nameof(stats));

            return stats.Select(s => new RawRankingInfo(s.Key, s.Value.ItemPickedUp.Values.SumLong())).Ranking();
        }

        public static RankingInfo[] GetItemPickedUpRanking(this IReadOnlyDictionary<Guid, PlayerStats> stats, string key)
        {
            ArgumentNullException.ThrowIfNull(stats, nameof(stats));
            ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

            return stats.Select(s => new RawRankingInfo(s.Key, s.Value.ItemPickedUp.GetValueOrDefault(key, 0))).Ranking();
        }

        public static RankingInfo[] GetItemDroppedRanking(this IReadOnlyDictionary<Guid, PlayerStats> stats)
        {
            ArgumentNullException.ThrowIfNull(stats, nameof(stats));

            return stats.Select(s => new RawRankingInfo(s.Key, s.Value.ItemDropped.Values.SumLong())).Ranking();
        }

        public static RankingInfo[] GetItemDroppedRanking(this IReadOnlyDictionary<Guid, PlayerStats> stats, string key)
        {
            ArgumentNullException.ThrowIfNull(stats, nameof(stats));
            ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

            return stats.Select(s => new RawRankingInfo(s.Key, s.Value.ItemDropped.GetValueOrDefault(key, 0))).Ranking();
        }

        public static RankingInfo[] GetItemUsedRanking(this IReadOnlyDictionary<Guid, PlayerStats> stats)
        {
            ArgumentNullException.ThrowIfNull(stats, nameof(stats));

            return stats.Select(s => new RawRankingInfo(s.Key, s.Value.ItemUsed.Values.SumLong())).Ranking();
        }

        public static RankingInfo[] GetItemUsedRanking(this IReadOnlyDictionary<Guid, PlayerStats> stats, string key)
        {
            ArgumentNullException.ThrowIfNull(stats, nameof(stats));
            ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

            return stats.Select(s => new RawRankingInfo(s.Key, s.Value.ItemUsed.GetValueOrDefault(key, 0))).Ranking();
        }

        public static RankingInfo[] GetItemCraftedRanking(this IReadOnlyDictionary<Guid, PlayerStats> stats)
        {
            ArgumentNullException.ThrowIfNull(stats, nameof(stats));

            return stats.Select(s => new RawRankingInfo(s.Key, s.Value.ItemCrafted.Values.SumLong())).Ranking();
        }

        public static RankingInfo[] GetItemCraftedRanking(this IReadOnlyDictionary<Guid, PlayerStats> stats, string key)
        {
            ArgumentNullException.ThrowIfNull(stats, nameof(stats));
            ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

            return stats.Select(s => new RawRankingInfo(s.Key, s.Value.ItemCrafted.GetValueOrDefault(key, 0))).Ranking();
        }

        public static RankingInfo[] GetItemBrokenRanking(this IReadOnlyDictionary<Guid, PlayerStats> stats)
        {
            ArgumentNullException.ThrowIfNull(stats, nameof(stats));

            return stats.Select(s => new RawRankingInfo(s.Key, s.Value.ItemBroken.Values.SumLong())).Ranking();
        }

        public static RankingInfo[] GetItemBrokenRanking(this IReadOnlyDictionary<Guid, PlayerStats> stats, string key)
        {
            ArgumentNullException.ThrowIfNull(stats, nameof(stats));
            ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

            return stats.Select(s => new RawRankingInfo(s.Key, s.Value.ItemBroken.GetValueOrDefault(key, 0))).Ranking();
        }

        public static RankingInfo[] GetBlockMinedRanking(this IReadOnlyDictionary<Guid, PlayerStats> stats)
        {
            ArgumentNullException.ThrowIfNull(stats, nameof(stats));

            return stats.Select(s => new RawRankingInfo(s.Key, s.Value.BlockMined.Values.SumLong())).Ranking();
        }

        public static RankingInfo[] GetBlockMinedRanking(this IReadOnlyDictionary<Guid, PlayerStats> stats, string key)
        {
            ArgumentNullException.ThrowIfNull(stats, nameof(stats));
            ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

            return stats.Select(s => new RawRankingInfo(s.Key, s.Value.BlockMined.GetValueOrDefault(key, 0))).Ranking();
        }

        public static RankingInfo[] GetEntityKilledRanking(this IReadOnlyDictionary<Guid, PlayerStats> stats)
        {
            ArgumentNullException.ThrowIfNull(stats, nameof(stats));

            return stats.Select(s => new RawRankingInfo(s.Key, s.Value.EntityKilled.Values.SumLong())).Ranking();
        }

        public static RankingInfo[] GetEntityKilledRanking(this IReadOnlyDictionary<Guid, PlayerStats> stats, string key)
        {
            ArgumentNullException.ThrowIfNull(stats, nameof(stats));
            ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

            return stats.Select(s => new RawRankingInfo(s.Key, s.Value.EntityKilled.GetValueOrDefault(key, 0))).Ranking();
        }

        public static RankingInfo[] GetEntityKilledByRanking(this IReadOnlyDictionary<Guid, PlayerStats> stats)
        {
            ArgumentNullException.ThrowIfNull(stats, nameof(stats));

            return stats.Select(s => new RawRankingInfo(s.Key, s.Value.EntityKilledBy.Values.SumLong())).Ranking();
        }

        public static RankingInfo[] GetEntityKilledByRanking(this IReadOnlyDictionary<Guid, PlayerStats> stats, string key)
        {
            ArgumentNullException.ThrowIfNull(stats, nameof(stats));
            ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

            return stats.Select(s => new RawRankingInfo(s.Key, s.Value.EntityKilledBy.GetValueOrDefault(key, 0))).Ranking();
        }

        public static RankingInfo[] GetTypedRanking(this IReadOnlyDictionary<Guid, PlayerStats> stats, string statsType)
        {
            ArgumentNullException.ThrowIfNull(stats, nameof(stats));
            ArgumentException.ThrowIfNullOrEmpty(statsType, nameof(statsType));

            if (!PlayerStats.StatsTypes.Contains(statsType))
                throw new ArgumentException("Invalid stats type.", nameof(statsType));

            return stats.Select(s => new RawRankingInfo(s.Key, s.Value.GetStatsFromType(statsType).Values.SumLong())).Ranking();
        }

        public static RankingInfo[] GetTypedRanking(this IReadOnlyDictionary<Guid, PlayerStats> stats, string statsType, string key)
        {
            ArgumentNullException.ThrowIfNull(stats, nameof(stats));
            ArgumentException.ThrowIfNullOrEmpty(statsType, nameof(statsType));
            ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

            if (!PlayerStats.StatsTypes.Contains(statsType))
                throw new ArgumentException("Invalid stats type.", nameof(statsType));

            return stats.Select(s => new RawRankingInfo(s.Key, s.Value.GetStatsFromType(statsType).GetValueOrDefault(key, 0))).Ranking();
        }

        public static long SumLong(this IEnumerable<int> source)
        {
            long sum = 0;

            foreach (int item in source)
                sum += item;

            return sum;
        }

        private static RankingInfo[] Ranking(this IEnumerable<RawRankingInfo> source)
        {
            ArgumentNullException.ThrowIfNull(source, nameof(source));

            int count = 0;
            return source
                .Where(s => s.Score > 0)
                .OrderByDescending(s => s.Score)
                .GroupBy(s => s.Score)
                .SelectMany(group =>
                {
                    // 当前组排名 = 之前累计的人数 + 1
                    int rank = count + 1;
                    var items = group.Select(s => new RankingInfo(s.Player, rank, s.Score));
                    count += group.Count(); // 累计这组的人数
                    return items;
                })
                .ToArray();
        }

        private record class RawRankingInfo(Guid Player, long Score);
    }
}
