using CoreRCON;
using QuanLib.Minecraft.Command.Model;
using QuanLib.Minecraft.Selectors;
using QuanLib.Minecraft.Snbt;
using QuanLib.Minecraft.Snbt.Model;
using QuanLib.Minecraft.Vector;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Nett.TomlObjectFactory;

namespace QuanLib.Minecraft
{
    [Obsolete("改用 CommandSender 与 CommandManager", true)]
    public class ServerCommandHelper
    {
        public ServerCommandHelper(RCON rcon)
        {
            _rcon = rcon ?? throw new ArgumentNullException(nameof(rcon));
            _semaphore = new(1);
        }

        private const string POS = "Pos";

        public const int DUAL_WIELD_SLOT = -106;

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

        public virtual string SendCommand(string command, bool retry = false, int maxRetrys = -1)
        {
            _semaphore.Wait();
            try
            {
                int retryCount = 0;
                retry:
                string output = _rcon.SendCommandAsync(command).Result;
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
            output = SendCommand(command, retry, maxRetrys);
            return !string.IsNullOrEmpty(output);
        }

        public virtual PlayerList GetPlayerList()
        {
            string output = SendCommand("list");
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

        public virtual int GetGameDays()
        {
            string output = SendCommand("time query day");
            return int.Parse(output.Split(' ')[^1]);
        }

        public virtual int GetDayTime()
        {
            string output = SendCommand("time query daytime");
            return int.Parse(output.Split(' ')[^1]);
        }

        public virtual int GetGameTime()
        {
            string output = SendCommand("time query gametime");
            return int.Parse(output.Split(' ')[^1]);
        }

        public virtual bool TelePort(Selector source, Selector target)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));
            if (target is null)
                throw new ArgumentNullException(nameof(target));

            string output = SendCommand($"tp {source} {target}");
            if (output.StartsWith("No entity was found"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public virtual bool TelePort(Selector source, double x, double y, double z)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            string output = SendCommand($"tp {source} {x} {y} {z}");
            if (output.StartsWith("No entity was found"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public virtual bool TelePort(Selector source, IVector3<double> target)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            string output = SendCommand($"tp {source} {target.X} {target.Y} {target.Z}");
            if (output.StartsWith("No entity was found"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public virtual bool TestBlcok(int x, int y, int z, string id)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            string output = SendCommand($"execute if block {x} {y} {z} {id}");
            if (output.StartsWith("Test passed"))
                return true;
            else
                return false;
        }

        public virtual bool TestBlcok(IVector3<int> pos, string id)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            string output = SendCommand($"execute if block {pos.X} {pos.Y} {pos.Z} {id}");

            if (output.StartsWith("Test passed"))
                return true;
            else
                return false;
        }

        public virtual int TestEntityCount(Selector target)
        {
            if (target is null)
                throw new ArgumentNullException(nameof(target));

            string output = SendCommand($"execute if entity {target}");
            if (int.TryParse(output.Split(' ')[^1], out int value))
                return value;
            else
                return 0;
        }

        public virtual bool TestEntity(Selector target)
        {
            return TestEntityCount(target) > 0;
        }

        public virtual bool SetBlock(int x, int y, int z, string id)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            string output = SendCommand($"setblock {x} {y} {z} {id}");

            if (output.StartsWith("Changed the block at"))
                return true;
            else
                return false;
        }

        public virtual bool SetBlock(IVector3<int> pos, string id)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            string output = SendCommand($"setblock {pos.X} {pos.Y} {pos.Z} {id}");

            if (output.StartsWith("Changed the block at"))
                return true;
            else
                return false;
        }

        public virtual bool SummonEntity(string id, double x, double y, double z, string? nbt = null)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            string command = $"summon {id} {x} {y} {z}";
            if (!string.IsNullOrEmpty(nbt))
                command += ' ' + nbt;

            string output = SendCommand(command);
            if (output.StartsWith("Summoned new"))
                return true;
            else
                return false;
        }

        public virtual bool SummonEntity(string id, IVector3<double> pos, string? nbt = null)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            string command = $"summon {id} {pos.X} {pos.Y} {pos.Z}";
            if (!string.IsNullOrEmpty(nbt))
                command += ' ' + nbt;

            string output = SendCommand(command);
            if (output.StartsWith("Summoned new"))
                return true;
            else
                return false;
        }

        public virtual bool KillEntity(Selector target)
        {
            if (target is null)
                throw new ArgumentNullException(nameof(target));

            string output = SendCommand($"kill {target}");
            if (output.StartsWith("Killed"))
                return true;
            else
                return false;
        }

        public virtual void SendChatMessage(Selector target, string text, TextColor color = TextColor.White, bool bold = false)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;
            SendCommand($"tellraw {target} {CommandUtil.ToJson(text, color, bold)}");
        }

        #region 发送title

        public virtual void SendTitle(Selector target, string text, TextColor color = TextColor.White, bool bold = false)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;
            SendCommand($"title {target} title {CommandUtil.ToJson(text, color, bold)}");
        }

        public virtual void SendTitle(Selector target, int fadeIn, int stay, int fadeOut, string text, TextColor color = TextColor.White, bool bold = false)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;
            SendCommand($"title {target} times {fadeIn} {stay} {fadeOut}");
            SendCommand($"title {target} title {CommandUtil.ToJson(text, color, bold)}");
        }

        public virtual void SendSubTitle(Selector target, string text, TextColor color = TextColor.White, bool bold = false)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;
            SendCommand($"title {target} subtitle {CommandUtil.ToJson(text, color, bold)}");
        }

        public virtual void SendSubTitle(Selector target, int fadeIn, int stay, int fadeOut, string text, TextColor color = TextColor.White, bool bold = false)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;
            SendCommand($"title {target} times {fadeIn} {stay} {fadeOut}");
            SendCommand($"title {target} subtitle {CommandUtil.ToJson(text, color, bold)}");
        }

        public virtual void SendActionbarTitle(Selector target, string text, TextColor color = TextColor.White, bool bold = false)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;
            SendCommand($"title {target} actionbar {CommandUtil.ToJson(text, color, bold)}");
        }

        //public virtual void SendActionbarTitle(Selector target, int fadeIn, int stay, int fadeOut, string text, TextColor color = TextColor.White, bool bold = false)
        //{
        //    if (string.IsNullOrWhiteSpace(text))
        //        return;
        //    SendCommand($"title {target} times {fadeIn} {stay} {fadeOut}");
        //    SendCommand($"title {target} actionbar {CommandUtil.ToJson(text, color, bold)}");
        //}

        public virtual void SetTitleTimes(Selector target, int fadeIn, int stay, int fadeOut)
        {
            SendCommand($"title {target} times {fadeIn} {stay} {fadeOut}");
        }

        public virtual void ClearTitle(Selector target)
        {
            SendCommand($"title {target} clear");
        }

        public virtual void ResetTitle(Selector target)
        {
            SendCommand($"title {target} reset");
        }

        #endregion

        #region 获取SNBT

        public virtual PlayerEntity GetPlayerEntityData(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException($"“{nameof(name)}”不能为 null 或空。", nameof(name));

            string output = SendCommand("data get entity " + name);
            return new(SnbtSerializer.DeserializeObject<PlayerEntity.Model>(SplitEntitySnbt(output)));
        }

        public virtual bool TryGetPlayerEntityData(string name, [MaybeNullWhen(false)] out PlayerEntity result)
        {
            if (string.IsNullOrEmpty(name))
                goto err;

            string output = SendCommand("data get entity " + name);
            if (output.StartsWith("No entity was found"))
                goto err;

            result = new(SnbtSerializer.DeserializeObject<PlayerEntity.Model>(SplitEntitySnbt(output)));
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

            result = new(SnbtSerializer.DeserializeObject<Item.Model>(snbt));
            return true;

            err:
            result = default;
            return false;
        }

        public virtual Dictionary<string, Item> GetAllPlayerItem(Dictionary<string, int> slots)
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
            return TryGetPlayerItem(name, DUAL_WIELD_SLOT, out result);
        }

        public virtual bool SetPlayerHotbarItem(string name, int slot, string itemID)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException($"“{nameof(name)}”不能为 null 或空。", nameof(name));
            if (string.IsNullOrEmpty(itemID))
                throw new ArgumentException($"“{nameof(itemID)}”不能为 null 或空。", nameof(itemID));

            itemID = itemID.Replace("\\", "\\\\");
            string output = SendCommand($"item replace entity {name} hotbar.{slot} with {itemID}");
            if (output.StartsWith("Replaced a slot on"))
                return true;
            else
                return false;
        }

        public virtual bool TryGetEntityPosition(string target, out EntityPos result)
        {
            if (!TryGetEntitySnbt(target, POS, out var snbt))
            {
                result = default;
                return false;
            }

            return MinecraftUtil.TryParseEntityPosSbnt(snbt, out result);
        }

        public virtual bool TryGetEntityRotation(string target, out Rotation result)
        {
            if (!TryGetEntitySnbt(target, "Rotation", out var snbt))
            {
                result = default;
                return false;
            }
            return MinecraftUtil.TryParseRotationSbnt(snbt, out result);
        }

        public virtual bool TryGetEntityUUID(string target, out Guid result)
        {
            if (!TryGetEntitySnbt(target, "UUID", out var snbt))
            {
                result = default;
                return false;
            }
            return MinecraftUtil.TryParseUuidSbnt(snbt, out result);
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

        public virtual LeftRightKeys GetInteractionData(string target)
        {
            if (string.IsNullOrEmpty(target))
                throw new ArgumentException($"“{nameof(target)}”不能为 null 或空。", nameof(target));

            InteractionData leftData, rightData;
            if (TryGetEntitySnbt(target, "attack", out var left))
                leftData = new(SnbtSerializer.DeserializeObject<InteractionData.Model>(left));
            else
                leftData = new(Guid.Empty, 0);
            if (TryGetEntitySnbt(target, "interaction", out var right))
                rightData = rightData = new(SnbtSerializer.DeserializeObject<InteractionData.Model>(right));
            else
                rightData = new(Guid.Empty, 0);

            return new(leftData, rightData);
        }

        public virtual string GetAllEntitySbnt(string target, string? path)
        {
            if (string.IsNullOrEmpty(target))
                throw new ArgumentException($"“{nameof(target)}”不能为 null 或空。", nameof(target));

            string command = $"execute as {target} run data get entity @s";
            if (!string.IsNullOrEmpty(path))
                command += ' ' + path;
            return SendCommand(command);
        }

        public virtual Dictionary<string, string> GetAllPlayerSbnt(string target, string? path)
        {
            if (string.IsNullOrEmpty(target))
                throw new ArgumentException($"“{nameof(target)}”不能为 null 或空。", nameof(target));

            Dictionary<string, string> result = new();
            PlayerList list = GetPlayerList();
            if (list.List.Count == 0)
                return result;

            string command = $"execute as {target} run data get entity @s";
            if (!string.IsNullOrEmpty(path))
                command += ' ' + path;
            string output = SendCommand(command);
            if (string.IsNullOrEmpty(output))
                return result;

            List<int> indexs = new();
            foreach (var player in list.List)
            {
                int index = output.IndexOf(player + " has the following");
                if (index != -1)
                    indexs.Add(index);
            }
            indexs.Sort();

            List<string> items = new();
            int current = 0;
            foreach (var index in indexs)
            {
                if (index <= current)
                    continue;

                items.Add(output[current..index]);
                current = index;
            }
            if (current < output.Length)
                items.Add(output[current..output.Length]);

            foreach (string item in items)
                result.Add(item.Split(' ')[0], SplitEntitySnbt(item));
            return result;
        }

        /// <summary>
        /// 获取所有玩家的坐标
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<string, IVector3<double>> GetAllPlayerPosition()
        {
            Dictionary<string, string> items = GetAllPlayerSbnt("@a", POS);
            Dictionary<string, IVector3<double>> result = new();
            foreach (var item in items)
            {
                if (MinecraftUtil.TryParseEntityPosSbnt(item.Value, out var position))
                    result.Add(item.Key, position);
            }
            return result;
        }

        /// <summary>
        /// 获取球形半径内所有玩家的坐标
        /// </summary>
        /// <param name="centre">中心点坐标</param>
        /// <param name="radius">半径</param>
        /// <returns></returns>
        public virtual Dictionary<string, IVector3<double>> GetRadiusPlayerPosition(EntityPos centre, int radius)
        {
            Dictionary<string, string> items = GetAllPlayerSbnt(ToTarget("a", centre, radius), POS);
            Dictionary<string, IVector3<double>> result = new();
            foreach (var item in items)
            {
                if (MinecraftUtil.TryParseEntityPosSbnt(item.Value, out var position))
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
        public virtual Dictionary<string, IVector3<double>> GetRangePlayerPosition(IVector3<double> start, IVector3<double> range)
        {
            Dictionary<string, string> items = GetAllPlayerSbnt(ToTarget("a", start, range), POS);
            Dictionary<string, IVector3<double>> result = new();
            foreach (var item in items)
            {
                if (MinecraftUtil.TryParseEntityPosSbnt(item.Value, out var position))
                    result.Add(item.Key, position);
            }
            return result;
        }

        public virtual Dictionary<string, int> GetAllPlayerSelectedItemSlot()
        {
            Dictionary<string, string> items = GetAllPlayerSbnt("@a", "SelectedItemSlot");
            Dictionary<string, int> result = new();
            foreach (var item in items)
            {
                if (int.TryParse(item.Value, out var slot))
                    result.Add(item.Key, slot);
            }
            return result;
        }

        public virtual Dictionary<string, int> GetRadiusPlayerSelectedItemSlot(IVector3<double> centre, int radius)
        {
            Dictionary<string, string> items = GetAllPlayerSbnt(ToTarget("a", centre, radius), "SelectedItemSlot");
            Dictionary<string, int> result = new();
            foreach (var item in items)
            {
                if (int.TryParse(item.Value, out var slot))
                    result.Add(item.Key, slot);
            }
            return result;
        }

        public virtual Dictionary<string, Item> GetAllPlayerItem(int slot)
        {
            Dictionary<string, string> items = GetAllPlayerSbnt("@a", $"Inventory[{{Slot:{slot}b}}]");
            Dictionary<string, Item> result = new();
            foreach (var item in items)
            {
                result.Add(item.Key, new(SnbtSerializer.DeserializeObject<Item.Model>(item.Value)));
            }
            return result;
        }

        public virtual Dictionary<string, Item> GetRadiusPlayerItem(EntityPos centre, int radius, int slot)
        {
            Dictionary<string, string> items = GetAllPlayerSbnt(ToTarget("a", centre, radius), $"Inventory[{{Slot:{slot}b}}]");
            Dictionary<string, Item> result = new();
            foreach (var item in items)
            {
                result.Add(item.Key, new(SnbtSerializer.DeserializeObject<Item.Model>(item.Value)));
            }
            return result;
        }

        public virtual Dictionary<string, Item> GetAllPlayerSelectedItem()
        {
            return GetAllPlayerItem(GetAllPlayerSelectedItemSlot());
        }

        public virtual Dictionary<string, Item> GetRadiusPlayerSelectedItem(EntityPos centre, int radius)
        {
            return GetAllPlayerItem(GetRadiusPlayerSelectedItemSlot(centre, radius));
        }

        public virtual Dictionary<string, Item> GetAllPlayerDualWieldItem()
        {
            return GetAllPlayerItem(DUAL_WIELD_SLOT);
        }

        public virtual Dictionary<string, Item> GetRadiusPlayerDualWieldItem(EntityPos centre, int radius)
        {
            return GetRadiusPlayerItem(centre, radius, DUAL_WIELD_SLOT);
        }

        public virtual bool TryGetPlayerScoreboard(string playerName, string scoreboardName, out int result)
        {
            if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(scoreboardName))
                goto err;

            string output = SendCommand($"scoreboard players get {playerName} {scoreboardName}");

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

        public virtual bool SetEntityNbt(string target, string path, string value)
        {
            if (string.IsNullOrEmpty(target) || string.IsNullOrEmpty(path) || string.IsNullOrEmpty(value))
                return false;

            string output = SendCommand($"data modify entity {target} {path} set value {value}");
            if (output.StartsWith("Modified entity data of"))
                return true;
            else
                return false;
        }

        public virtual bool SetEntityHealth(string target, float value)
        {
            return SetEntityNbt(target, "Health", value.ToString());
        }

        #endregion

        #region 计分板

        public virtual string[] GetScoreboardList()
        {
            List<string> result = new();
            string output = SendCommand("scoreboard objectives list");
            MatchCollection matche = new Regex(@"\[(.*?)\]").Matches(output);
            foreach (Match match in matche.Cast<Match>())
                result.Add(match.Value);
            return result.ToArray();
        }

        public virtual bool SetPlayerScoreboard(string playerName, string scoreboardName, int score)
        {
            if (string.IsNullOrEmpty(playerName))
                throw new ArgumentException($"“{nameof(playerName)}”不能为 null 或空。", nameof(playerName));
            if (string.IsNullOrEmpty(scoreboardName))
                throw new ArgumentException($"“{nameof(scoreboardName)}”不能为 null 或空。", nameof(scoreboardName));

            string output = SendCommand($"scoreboard players set {playerName} {scoreboardName} {score}");
            if (output.StartsWith("Set"))
                return true;
            else
                return false;
        }

        public virtual bool CreatScoreboard(string scoreboardName, string criteria)
        {
            string output = SendCommand($"scoreboard objectives add {scoreboardName} {criteria}");
            if (output.StartsWith("Created new objective"))
                return true;
            else
                return false;
        }

        public virtual bool RemoveScoreboard(string scoreboardName)
        {
            string output = SendCommand($"scoreboard objectives remove {scoreboardName}");
            if (output.StartsWith("Removed objective"))
                return true;
            else
                return false;
        }

        #endregion

        public virtual bool AddForceLoadChunk(IVector3<int> blockPos)
        {
            string output = SendCommand($"forceload add {blockPos.X} {blockPos.Z}");
            if (output.StartsWith("Marked chunk"))
                return true;
            else
                return false;
        }

        public virtual bool RemoveForceLoadChunk(IVector3<int> blockPos)
        {
            string output = SendCommand($"forceload remove {blockPos.X} {blockPos.Z}");
            if (output.StartsWith("Unmarked chunk"))
                return true;
            else
                return false;
        }

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
