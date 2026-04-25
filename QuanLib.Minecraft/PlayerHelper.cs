using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace QuanLib.Minecraft
{
    public static class PlayerHelper
    {
        public static Dictionary<Guid, string> ParseUserNameCache(string json)
        {
            ArgumentException.ThrowIfNullOrEmpty(json, nameof(json));

            return JsonSerializer.Deserialize<Dictionary<Guid, string>>(json) ?? [];
        }

        public static PlayerCache[] ParseUserCache(string json)
        {
            ArgumentException.ThrowIfNullOrEmpty(json, nameof(json));

            var models = JsonSerializer.Deserialize<PlayerCache.Model[]>(json);
            return models?.Select(model => new PlayerCache(model)).ToArray() ?? [];
        }

        public static PlayerInfo[] ParseWhitelist(string json)
        {
            ArgumentException.ThrowIfNullOrEmpty(json, nameof(json));

            var models = JsonSerializer.Deserialize<PlayerInfo.Model[]>(json);
            return models?.Select(model => new PlayerInfo(model)).ToArray() ?? [];
        }

        public static BannedPlayer[] ParseBannedPlayers(string json)
        {
            ArgumentException.ThrowIfNullOrEmpty(json, nameof(json));

            var models = JsonSerializer.Deserialize<BannedPlayer.Model[]>(json);
            return models?.Select(model => new BannedPlayer(model)).ToArray() ?? [];
        }

        public static BannedIpAddress[] ParseBannedIpAddress(string json)
        {
            ArgumentException.ThrowIfNullOrEmpty(json, nameof(json));

            var models = JsonSerializer.Deserialize<BannedIpAddress.Model[]>(json);
            return models?.Select(model => new BannedIpAddress(model)).ToArray() ?? [];
        }
    }
}
