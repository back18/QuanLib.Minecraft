using CoreRCON;
using QuanLib.Minecraft.Datas;
using QuanLib.Minecraft.Selectors;
using QuanLib.Minecraft.Snbt;
using QuanLib.Minecraft.Vectors;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuanLib.Minecraft
{
    public class ServerCommandHelper
    {
        public ServerCommandHelper(RCON rcon)
        {
            _rcon = rcon ?? throw new ArgumentNullException(nameof(rcon));
            _semaphore = new(1);
        }

        private const string POS = "Pos";

        private readonly RCON _rcon;

        private readonly SemaphoreSlim _semaphore;

        public virtual async Task<string> SendCommandAsync(string command, bool retry = false, int maxRetrys = -1)
        {
            await _semaphore.WaitAsync();
            try
            {
                int retryCount = 0;
                retry:
                string output = await _rcon.SendCommandAsync(command);
                if (retry && string.IsNullOrEmpty(output) && (maxRetrys == -1 || retryCount < maxRetrys))
                {
                    retryCount++;
                    goto retry;
                }
                return output;
            }
            catch
            {
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public virtual bool TrySendCommand(string command, [MaybeNullWhen(false)] out string output, bool retry = false, int maxRetrys = -1)
        {
            output = SendCommandAsync(command, retry, maxRetrys).Result;
            return !string.IsNullOrEmpty(output);
        }

        public virtual async Task<PlayerList> GetPlayerListAsync()
        {
            string output = await SendCommandAsync("list");
            Match match = Regex.Match(output, @"There are (\d+) of a max of (\d+) players online: (.+)");
            if (match.Success)
            {
                int onlinePlayers = int.Parse(match.Groups[1].Value);
                int maxPlayers = int.Parse(match.Groups[2].Value);
                string[] playerList = match.Groups[3].Value.Split(", ");
                return new(onlinePlayers, maxPlayers, playerList);
            }
            else
            {
                return new(0, 0, Array.Empty<string>());
            }
        }

        public virtual async Task<int> GetGameDays()
        {
            string output = await SendCommandAsync("time query day");
            return int.Parse(output.Split(' ')[^1]);
        }

        public virtual async Task<int> GetDayTime()
        {
            string output = await SendCommandAsync("time query daytime");
            return int.Parse(output.Split(' ')[^1]);
        }

        public virtual async Task<int> GetGameTime()
        {
            string output = await SendCommandAsync("time query gametime");
            return int.Parse(output.Split(' ')[^1]);
        }

        public virtual async Task<bool> TelePortAsync(string source, string target)
        {
            string output = await SendCommandAsync($"tp {source} {target}");
            if (output.StartsWith("No entity was found"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public virtual async Task<bool> TelePortAsync(string source, double x, double y, double z)
        {
            string output = await SendCommandAsync($"tp {source} {x} {y} {z}");
            if (output.StartsWith("No entity was found"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public virtual async Task<bool> TelePortAsync(string source, IVector3<double> target)
        {
            string output = await SendCommandAsync($"tp {source} {target.X} {target.Y} {target.Z}");
            if (output.StartsWith("No entity was found"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public virtual async Task<bool> SetBlockAsync(int x, int y, int z, string id)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            string output = await SendCommandAsync($"setblock {x} {y} {z} {id}");

            if (output.StartsWith("Changed the block at"))
                return true;
            else
                return false;
        }

        public virtual async Task<bool> SetBlockAsync(IVector3<int> pos, string id)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            string output = await SendCommandAsync($"setblock {pos.X} {pos.Y} {pos.Z} {id}");

            if (output.StartsWith("Changed the block at"))
                return true;
            else
                return false;
        }

        public virtual async Task SendChatMessageAsync(Selector target, string text, TextColor color = TextColor.White, bool bold = false)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;
            await SendCommandAsync($"tellraw {target} {CommandUtil.ToJson(text, color, bold)}");
        }

        #region 发送title

        public virtual async Task SendTitleAsync(Selector target, string text, TextColor color = TextColor.White, bool bold = false)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;
            await SendCommandAsync($"title {target} title {CommandUtil.ToJson(text, color, bold)}");
        }

        public virtual async Task SendSubTitleAsync(Selector target, string text, TextColor color = TextColor.White, bool bold = false)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;
            await SendCommandAsync($"title {target} subtitle {CommandUtil.ToJson(text, color, bold)}");
        }

        public virtual async Task SendActionbarTitleAsync(Selector target, string text, TextColor color = TextColor.White, bool bold = false)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;
            await SendCommandAsync($"title {target} actionbar {CommandUtil.ToJson(text, color, bold)}");
        }

        public virtual async Task SetTitleTimesAsync(Selector target, int fadeIn, int stay, int fadeOut)
        {
            await SendCommandAsync($"title {target} times {fadeIn} {stay} {fadeOut}");
        }

        public virtual async Task ClearTitleAsync(Selector target)
        {
            await SendCommandAsync($"title {target} clear");
        }

        public virtual async Task ResetTitleAsync(Selector target)
        {
            await SendCommandAsync($"title {target} reset");
        }

        #endregion

        #region 获取SNBT

        public virtual async Task<PlayerEntity> GetPlayerEntityDataAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException($"“{nameof(name)}”不能为 null 或空。", nameof(name));

            string output = await SendCommandAsync("data get entity " + name);
            return new(SnbtSerializer.DeserializeObject<PlayerEntity.Nbt>(SplitEntitySnbt(output)));
        }

        public virtual bool TryGetPlayerEntityData(string name, [MaybeNullWhen(false)] out PlayerEntity result)
        {
            if (string.IsNullOrEmpty(name))
                goto err;

            string output = SendCommandAsync("data get entity " + name).Result;
            if (output.StartsWith("No entity was found"))
                goto err;

            result = new(SnbtSerializer.DeserializeObject<PlayerEntity.Nbt>(SplitEntitySnbt(output)));
            return true;

            err:
            result = default;
            return false;
        }

        public virtual bool TryGetEntitySnbt(string target, string? path, [MaybeNullWhen(false)] out string result)
        {
            if (string.IsNullOrEmpty(target))
                goto err;

            string command = $"data get entity {target}";
            if (!string.IsNullOrEmpty(path))
                command += ' ' + path;
            if (!TrySendCommand(command, out var output) ||
                output.StartsWith("No entity was found") ||
                output.StartsWith("Found no elements matching"))
                goto err;

            result = SplitEntitySnbt(output);
            return true;

            err:
            result = default;
            return false;
        }

        public virtual bool TryGetPlayerSelectedItemSlot(string name, out int result)
        {
            if (!TryGetEntitySnbt(name, "SelectedItemSlot", out var snbt))
            {
                result = default;
                return false;
            }

            return int.TryParse(snbt, out result);
        }

        public virtual bool TryGetPlayerItem(string name, int slot, [MaybeNullWhen(false)] out Item result)
        {
            if (!TryGetEntitySnbt(name, $"Inventory[{{Slot:{slot}b}}]", out var snbt))
                goto err;
            else if (snbt.StartsWith("Found no elements matching Inventory"))
                goto err;

            result = new(SnbtSerializer.DeserializeObject<Item.Nbt>(snbt));
            return true;

            err:
            result = default;
            return false;
        }

        public virtual Dictionary<string, Item> GetPlayersItem(Dictionary<string, int> slots)
        {
            Dictionary<string, Item> result = new();
            foreach (var slot in slots)
            {
                if (TryGetPlayerItem(slot.Key, slot.Value, out var item))
                    result.Add(slot.Key, item);
            }
            return result;
        }

        public virtual bool TryGetPlayerSelectedItem(string name, [MaybeNullWhen(false)] out Item result)
        {
            if (!TryGetPlayerSelectedItemSlot(name, out var slot))
            {
                result = default;
                return false;
            }

            return TryGetPlayerItem(name, slot, out result);
        }

        public virtual bool TryGetPlayerDualWieldItem(string name, [MaybeNullWhen(false)] out Item result)
        {
            return TryGetPlayerItem(name, -106, out result);
        }

        public virtual async Task<bool> SetPlayerHotbarItemAsync(string name, int slot, string itemID)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException($"“{nameof(name)}”不能为 null 或空。", nameof(name));
            if (string.IsNullOrEmpty(itemID))
                throw new ArgumentException($"“{nameof(itemID)}”不能为 null 或空。", nameof(itemID));

            string output = await SendCommandAsync($"item replace entity {name} hotbar.{slot} with {itemID}");
            if (output.StartsWith("Replaced a slot on"))
                return true;
            else
                return false;
        }

        public virtual bool TryGetEntityPosition(string target, out Vector3Double result)
        {
            if (!TryGetEntitySnbt(target, POS, out var snbt))
            {
                result = default;
                return false;
            }

            return MinecraftUtil.TryEntityPositionSbnt(snbt, out result);
        }

        public virtual bool TryGetEntityRotation(string target, out Rotation result)
        {
            if (!TryGetEntitySnbt(target, "Rotation", out var snbt))
            {
                result = default;
                return false;
            }
            return Rotation.TryParse(snbt, out result);
        }
        
        public virtual bool TryGetEntityHealth(string target, out float result)
        {
            if (!TryGetEntitySnbt(target, "Health", out var snbt))
            {
                result = default;
                return false;
            }
            return float.TryParse(snbt[..^1], out result);
        }

        public virtual async Task<Dictionary<string, string>> GetPlayersSbnt(string target, string path)
        {
            if (string.IsNullOrEmpty(target))
                throw new ArgumentException($"“{nameof(target)}”不能为 null 或空。", nameof(target));

            string command = $"execute as {target} run data get entity @s";
            if (!string.IsNullOrEmpty(path))
                command += ' ' + path;
            string output = await SendCommandAsync(command);

            string[] items = output.Split(']', StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, string> result = new();
            foreach (string item in items)
                result.Add(item.Split(' ')[0], SplitEntitySnbt(item + ']'));
            return result;
        }

        /// <summary>
        /// 获取球形半径内所有玩家的坐标
        /// </summary>
        /// <param name="centre">中心点坐标</param>
        /// <param name="radius">半径</param>
        /// <returns></returns>
        public virtual async Task<Dictionary<string, IVector3<double>>> GetRadiusPlayerPositionAsync(Vector3Double centre, int radius)
        {
            Dictionary<string, string> items = await GetPlayersSbnt(ToTarget("a", centre, radius), POS);
            Dictionary<string, IVector3<double>> result = new();
            foreach (var item in items)
            {
                if (MinecraftUtil.TryEntityPositionSbnt(item.Value, out var position))
                    result.Add(item.Key, position);
            }
            return result;
        }

        /// <summary>
        /// 获取矩形范围内所有玩家的坐标
        /// </summary>
        /// <param name="start">起始点坐标</param>
        /// <param name="range">范围</param>
        /// <returns></returns>
        public virtual async Task<Dictionary<string, IVector3<double>>> GetRangePlayerPositionAsync(IVector3<double> start, IVector3<double> range)
        {
            Dictionary<string, string> items = await GetPlayersSbnt(ToTarget("a", start, range), POS);
            Dictionary<string, IVector3<double>> result = new();
            foreach (var item in items)
            {
                if (MinecraftUtil.TryEntityPositionSbnt(item.Value, out var position))
                    result.Add(item.Key, position);
            }
            return result;
        }

        public virtual async Task<Dictionary<string, int>> GetRadiusPlayerSelectedItemSlotAsync(IVector3<double> centre, int radius)
        {
            Dictionary<string, string> items = await GetPlayersSbnt(ToTarget("a", centre, radius), "SelectedItemSlot");
            Dictionary<string, int> result = new();
            foreach (var item in items)
            {
                if (int.TryParse(item.Value, out var slot))
                    result.Add(item.Key, slot);
            }
            return result;
        }

        public virtual async Task<Dictionary<string, Item>> GetRadiusPlayerItemAsync(Vector3Double centre, int radius, int slot)
        {
            Dictionary<string, string> items = await GetPlayersSbnt(ToTarget("a", centre, radius), $"Inventory[{{Slot:{slot}b}}]");
            Dictionary<string, Item> result = new();
            foreach (var item in items)
            {
                result.Add(item.Key, new(SnbtSerializer.DeserializeObject<Item.Nbt>(item.Value)));
            }
            return result;
        }

        public virtual async Task<Dictionary<string, Item>> GetRadiusPlayerSelectedItemAsync(Vector3Double centre, int radius)
        {
            return GetPlayersItem(await GetRadiusPlayerSelectedItemSlotAsync(centre, radius));
        }

        public virtual async Task<Dictionary<string, Item>> GetRadiusPlayerDualWieldItemAsync(Vector3Double centre, int radius)
        {
            return await GetRadiusPlayerItemAsync(centre, radius, -106);
        }

        public virtual bool TryGetPlayerScoreboard(string playerName, string scoreboardName, out int result)
        {
            if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(scoreboardName))
                goto err;

            string output = SendCommandAsync($"scoreboard players get {playerName} {scoreboardName}").Result;

            if (output.StartsWith("No entity was found") ||
                output.StartsWith("Can't get value") ||
                output.StartsWith("Unknown scoreboard objective"))
                goto err;

            Match match = Regex.Match(output, @"(?<=has\s)\d+");
            if (!match.Success || !int.TryParse(match.Value, out result))
                goto err;

            return true;

            err:
            result = default;
            return false;
        }

        #endregion

        #region 设置SNBT

        public virtual async Task<bool> SetEntityNbtAsync(string target, string path, string value)
        {
            if (string.IsNullOrEmpty(target) || string.IsNullOrEmpty(path) || string.IsNullOrEmpty(value))
                return false;

            string output = await SendCommandAsync($"data modify entity {target} {path} set value {value}");
            if (output.StartsWith("Modified entity data of"))
                return true;
            else
                return false;
        }

        public virtual async Task<bool> SetEntityHealthAsync(string target, float value)
        {
            return await SetEntityNbtAsync(target, "Health", value.ToString());
        }

        #endregion

        #region 计分板

        public virtual async Task<string[]> GetScoreboardListAsync()
        {
            List<string> result = new();
            string output = await SendCommandAsync("scoreboard objectives list");
            MatchCollection matche = new Regex(@"\[(.*?)\]").Matches(output);
            foreach (Match match in matche.Cast<Match>())
                result.Add(match.Value);
            return result.ToArray();
        }

        public virtual async Task<bool> SetPlayerScoreboardAsync(string playerName, string scoreboardName, int score)
        {
            if (string.IsNullOrEmpty(playerName))
                throw new ArgumentException($"“{nameof(playerName)}”不能为 null 或空。", nameof(playerName));
            if (string.IsNullOrEmpty(scoreboardName))
                throw new ArgumentException($"“{nameof(scoreboardName)}”不能为 null 或空。", nameof(scoreboardName));

            string output = await SendCommandAsync($"scoreboard players set {playerName} {scoreboardName} {score}");
            if (output.StartsWith("Set"))
                return true;
            else
                return false;
        }

        public virtual async Task<bool> CreatScoreboard(string scoreboardName, string criteria)
        {
            string output = await SendCommandAsync($"scoreboard objectives add {scoreboardName} {criteria}");
            if (output.StartsWith("Created new objective"))
                return true;
            else
                return false;
        }

        public virtual async Task<bool> RemoveScoreboard(string scoreboardName)
        {
            string output = await SendCommandAsync($"scoreboard objectives remove {scoreboardName}");
            if (output.StartsWith("Removed objective"))
                return true;
            else
                return false;
        }

        #endregion

        private static string SplitEntitySnbt(string output)
            => output.Split("entity data: ")[^1];

        private static string ToTarget(string at, IVector3<double> centre, int radius)
        {
            if (string.IsNullOrEmpty(at))
                throw new ArgumentException($"“{nameof(at)}”不能为 null 或空。", nameof(at));

            return $"@{at}[x={centre.X},y={centre.Y},z={centre.Z},distance=..{radius}]";
        }

        private static string ToTarget(string at, IVector3<double> start, IVector3<double> end)
        {
            if (string.IsNullOrEmpty(at))
                throw new ArgumentException($"“{nameof(at)}”不能为 null 或空。", nameof(at));

            return $"@{at}[x={start.X},y={start.Y},z={start.Z},dx={end.X},dy={end.Y},dz={end.Z}]";
        }
    }
}
