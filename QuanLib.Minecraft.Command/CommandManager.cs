using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuanLib.Minecraft.Command.Models;
using QuanLib.Minecraft.NBT.Models;
using QuanLib.Minecraft.Command.Senders;
using QuanLib.Minecraft.NBT;
using QuanLib.Minecraft.Vector;

namespace QuanLib.Minecraft.Command
{
    public static class CommandManager
    {
        public static readonly ListCommand ListCommand = new();
        public static readonly TimeQueryDayCommand TimeQueryDayCommand = new();
        public static readonly TimeQueryDaytimeCommand TimeQueryDaytimeCommand = new();
        public static readonly TimeQueryGametimeCommand TimeQueryGametimeCommand = new();
        public static readonly SetBlockCommand SetBlockCommand = new();
        public static readonly SummonCommand SummonCommand = new();
        public static readonly SummonHaveNbtCommand SummonHaveNbtCommand = new();
        public static readonly KillCommand KillCommand = new();
        public static readonly TelePortEntityCommand TelePortEntityCommand = new();
        public static readonly TelePortLocationCommand TelePortLocationCommand = new();
        public static readonly ConditionalBlockCommand ConditionalBlockCommand = new();
        public static readonly ConditionalEntityCommand ConditionalEntityCommand = new();
        public static readonly DataGetEntityCommand DataGetEntityCommand = new();
        public static readonly DataGetEntityHavePathCommand DataGetEntityHavePathCommand = new();
        public static readonly ItemReplaceWithEntityHotbarCommand ItemReplaceWithEntityHotbarCommand = new();
        public static readonly TellrawCommand TellrawCommand = new();
        public static readonly TitleTitleCommand TitleTitleCommand = new();
        public static readonly TitleSubTitleCommand TitleSubTitleCommand = new();
        public static readonly TitleActionbarCommand TitleActionbarCommand = new();
        public static readonly TitleTimesCommand TitleTimesCommand = new();
        public static readonly ScoreboardPlayersGetCommand ScoreboardPlayersGetCommand = new();
        public static readonly ScoreboardPlayersSetCommand ScoreboardPlayersSetCommand = new();
        public static readonly ForceloadAddCommand ForceloadAddCommand = new();
        public static readonly ForceloadRemoveCommand ForceloadRemoveCommand = new();

        public static SnbtCache SnbtCache { get; } = new(TimeSpan.FromMilliseconds(50));

        public static PlayerList GetPlayerList(this CommandSender sender)
        {
            return ListCommand.TrySendCommand(sender, out var result) ? result : PlayerList.Empty;
        }

        public static int GetGameDays(this CommandSender sender)
        {
            return TimeQueryDayCommand.TrySendCommand(sender, out var result) ? result : 0;
        }

        public static int GetDayTime(this CommandSender sender)
        {
            return TimeQueryDaytimeCommand.TrySendCommand(sender, out var result) ? result : 0;
        }

        public static int GetGameTime(this CommandSender sender)
        {
            return TimeQueryGametimeCommand.TrySendCommand(sender, out var result) ? result : 0;
        }

        public static bool SetBlock(this CommandSender sender, int x, int y, int z, string blockID)
        {
            return SetBlockCommand.TrySendCommand(sender, x, y, z, blockID);
        }

        public static bool SetBlock<T>(this CommandSender sender, T position, string blockID) where T : IVector3<int>
        {
            ArgumentNullException.ThrowIfNull(position, nameof(position));

            return SetBlockCommand.TrySendCommand(sender, position.X, position.Y, position.Z, blockID);
        }

        public static bool SummonEntity(this CommandSender sender, double x, double y, double z, string entityID, string? nbt = null)
        {
            if (string.IsNullOrEmpty(nbt))
                return SummonCommand.TrySendCommand(sender, x, y, z, entityID);
            else
                return SummonHaveNbtCommand.TrySendCommand(sender, x, y, z, entityID, nbt);
        }

        public static bool SummonEntity<T>(this CommandSender sender, T position, string entityID, string? nbt = null) where T : IVector3<double>
        {
            ArgumentNullException.ThrowIfNull(position, nameof(position));

            if (string.IsNullOrEmpty(nbt))
                return SummonCommand.TrySendCommand(sender, position.X, position.Y, position.Z, entityID);
            else
                return SummonHaveNbtCommand.TrySendCommand(sender, position.X, position.Y, position.Z, entityID, nbt);
        }

        public static int KillEntity(this CommandSender sender, string target)
        {
            return KillCommand.TrySendCommand(sender, target, out var result) ? result : 0;
        }

        public static int TelePort(this CommandSender sender, string source, string target)
        {
            return TelePortEntityCommand.TrySendCommand(sender, source, target, out var result) ? result : 0;
        }

        public static int TelePort(this CommandSender sender, string source, double x, double y, double z)
        {
            return TelePortLocationCommand.TrySendCommand(sender, source, x, y, z, out var result) ? result : 0;
        }

        public static int TelePort<T>(this CommandSender sender, string source, T position) where T : IVector3<double>
        {
            ArgumentNullException.ThrowIfNull(position, nameof(position));

            return TelePortLocationCommand.TrySendCommand(sender, source, position.X, position.Y, position.Z, out var result) ? result : 0;
        }

        public static bool ConditionalBlock(this CommandSender sender, int x, int y, int z, string blockID)
        {
            return ConditionalBlockCommand.TrySendCommand(sender, x, y, z, blockID);
        }

        public static bool ConditionalBlock<T>(this CommandSender sender, T position, string blockID) where T : IVector3<int>
        {
            ArgumentNullException.ThrowIfNull(position, nameof(position));

            return ConditionalBlockCommand.TrySendCommand(sender, position.X, position.Y, position.Z, blockID);
        }

        public static bool ConditionalEntity(this CommandSender sender, string target)
        {
            return ConditionalEntityCount(sender, target) > 0;
        }

        public static int ConditionalEntityCount(this CommandSender sender, string target)
        {
            return ConditionalEntityCommand.TrySendCommand(sender, target, out var result) ? result : 0;
        }

        #region GetSnbt

        public static bool TryGetEntitySnbt(this CommandSender sender, string target, [MaybeNullWhen(false)] out string result)
        {
            return DataGetEntityCommand.TrySendCommand(sender, target, out result);
        }

        public static bool TryGetEntitySnbt(this CommandSender sender, string target, string path, [MaybeNullWhen(false)] out string result)
        {
            return DataGetEntityHavePathCommand.TrySendCommand(sender, target, path, out result);
        }

        public static bool TryGetEntityPosition(this CommandSender sender, string target, out EntityPos result)
        {
            ArgumentException.ThrowIfNullOrEmpty(target, nameof(target));

            lock (SnbtCache.Position)
            {
                if (SnbtCache.Position.TryGetValue(target, out result))
                    return true;

                if (!TryGetEntitySnbt(sender, target, "Pos", out var snbt))
                {
                    result = default;
                    return false;
                }

                if (NbtUtil.TryParseEntityPosSbnt(snbt, out result))
                {
                    SnbtCache.Position[target] = result;
                    return true;
                }
                else
                {
                    return false;
                }
            }        }

        public static bool TryGetEntityRotation(this CommandSender sender, string target, out Rotation result)
        {
            ArgumentException.ThrowIfNullOrEmpty(target, nameof(target));

            lock (SnbtCache.Rotation)
            {
                if (SnbtCache.Rotation.TryGetValue(target, out result))
                    return true;

                if (!TryGetEntitySnbt(sender, target, "Rotation", out var snbt))
                {
                    result = default;
                    return false;
                }

                if (NbtUtil.TryParseRotationSbnt(snbt, out result))
                {
                    SnbtCache.Rotation[target] = result;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool TryGetEntityUuid(this CommandSender sender, string target, out Guid result)
        {
            ArgumentException.ThrowIfNullOrEmpty(target, nameof(target));

            lock (SnbtCache.Uuid)
            {
                if (SnbtCache.Uuid.TryGetValue(target, out result))
                    return true;

                if (!TryGetEntitySnbt(sender, target, "UUID", out var snbt))
                {
                    result = default;
                    return false;
                }

                if (NbtUtil.TryParseUuidSbnt(snbt, out result))
                {
                    SnbtCache.Uuid[target] = result;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool TryGetEntityHealth(this CommandSender sender, string target, out float result)
        {
            ArgumentException.ThrowIfNullOrEmpty(target, nameof(target));

            lock (SnbtCache.Health)
            {
                if (SnbtCache.Health.TryGetValue(target, out result))
                    return true;

                if (!TryGetEntitySnbt(sender, target, "Health", out var snbt))
                {
                    result = default;
                    return false;
                }

                if (float.TryParse(snbt[..^1], out result))
                {
                    SnbtCache.Health[target] = result;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool TryGetPlayerSelectedItemSlot(this CommandSender sender, string target, out int result)
        {
            ArgumentException.ThrowIfNullOrEmpty(target, nameof(target));

            lock (SnbtCache.SelectedItemSlot)
            {
                if (SnbtCache.SelectedItemSlot.TryGetValue(target, out result))
                    return true;

                if (!TryGetEntitySnbt(sender, target, "SelectedItemSlot", out var snbt))
                {
                    result = default;
                    return false;
                }

                if (int.TryParse(snbt, out result))
                {
                    SnbtCache.SelectedItemSlot[target] = result;
                    return true;
                }
                else
                {
                    return true;
                }
            }
        }

        public static bool TryGetPlayerItem(this CommandSender sender, string target, int slot, [MaybeNullWhen(false)] out Item result)
        {
            ArgumentException.ThrowIfNullOrEmpty(target, nameof(target));

            lock (SnbtCache.SelectedItemSlot)
            {
                if (SnbtCache.SelectedItem.TryGetValue(target, out var selectedItem) && slot == selectedItem.Slot)
                {
                    result = selectedItem;
                    return true;
                }
                else if (SnbtCache.DualWieldItem.TryGetValue(target, out var dualWieldItem) && slot == -106)
                {
                    result = dualWieldItem;
                    return true;
                }

                if (!TryGetEntitySnbt(sender, target, $"Inventory[{{Slot:{slot}b}}]", out var snbt))
                {
                    result = default;
                    return false;
                }

                result = new(NbtConvert.DeserializeObject<Item.Model>(snbt));
                if (SnbtCache.SelectedItemSlot.ContainsKey(target))
                    SnbtCache.SelectedItem[target] = result;
                else if (slot == -106)
                    SnbtCache.DualWieldItem[target] = result;

                return true;
            }
        }

        public static bool TryGetPlayerSelectedItem(this CommandSender sender, string target, [MaybeNullWhen(false)] out Item result)
        {
            if (!TryGetPlayerSelectedItemSlot(sender, target, out var slot))
            {
                result = default;
                return false;
            }

            return TryGetPlayerItem(sender, target, slot, out result);
        }

        public static bool TryGetPlayerDualWieldItem(this CommandSender sender, string target, [MaybeNullWhen(false)] out Item result)
        {
            return TryGetPlayerItem(sender, target, -106, out result);
        }

        public static LeftRightKeys GetInteractionData(this CommandSender sender, string target)
        {
            ArgumentException.ThrowIfNullOrEmpty(target, nameof(target));

            InteractionData leftData, rightData;
            if (TryGetEntitySnbt(sender, target, "attack", out var left))
                leftData = new(NbtConvert.DeserializeObject<InteractionData.Model>(left));
            else
                leftData = InteractionData.Empty;
            if (TryGetEntitySnbt(sender, target, "interaction", out var right))
                rightData = new(NbtConvert.DeserializeObject<InteractionData.Model>(right));
            else
                rightData = InteractionData.Empty;

            return new(leftData, rightData);
        }

        #endregion

        #region GetAllSnbt

        public static Dictionary<string, EntityPos> GetAllEntityPosition(this CommandSender sender, IEnumerable<string> targets)
        {
            ArgumentNullException.ThrowIfNull(targets, nameof(targets));

            Dictionary<string, EntityPos> result = new();
            foreach (var target in targets)
            {
                if (TryGetEntityPosition(sender, target, out var entityPos))
                    result.Add(target, entityPos);
            }

            return result;
        }

        public static Dictionary<string, Rotation> GetAllEntityRotation(this CommandSender sender, IEnumerable<string> targets)
        {
            ArgumentNullException.ThrowIfNull(targets, nameof(targets));

            Dictionary<string, Rotation> result = new();
            foreach (var target in targets)
            {
                if (TryGetEntityRotation(sender, target, out var rotation))
                    result.Add(target, rotation);
            }

            return result;
        }

        public static Dictionary<string, Guid> GetAllEntityUuid(this CommandSender sender, IEnumerable<string> targets)
        {
            ArgumentNullException.ThrowIfNull(targets, nameof(targets));

            Dictionary<string, Guid> result = new();
            foreach (var target in targets)
            {
                if (TryGetEntityUuid(sender, target, out var uuid))
                    result.Add(target, uuid);
            }

            return result;
        }

        public static Dictionary<string, float> GetAllEntityHealth(this CommandSender sender, IEnumerable<string> targets)
        {
            ArgumentNullException.ThrowIfNull(targets, nameof(targets));

            Dictionary<string, float> result = new();
            foreach (var target in targets)
            {
                if (TryGetEntityHealth(sender, target, out var health))
                    result.Add(target, health);
            }

            return result;
        }

        public static Dictionary<string, Item> GetAllPlayerSelectedItem(this CommandSender sender, IEnumerable<string> targets)
        {
            ArgumentNullException.ThrowIfNull(targets, nameof(targets));

            Dictionary<string, Item> result = new();
            foreach (var target in targets)
            {
                if (TryGetPlayerSelectedItem(sender, target, out var selectedItem))
                    result.Add(target, selectedItem);
            }

            return result;
        }

        public static Dictionary<string, Item> GetAllPlayerDualWieldItem(this CommandSender sender, IEnumerable<string> targets)
        {
            ArgumentNullException.ThrowIfNull(targets, nameof(targets));

            Dictionary<string, Item> result = new();
            foreach (var target in targets)
            {
                if (TryGetPlayerDualWieldItem(sender, target, out var dualWieldItem))
                    result.Add(target, dualWieldItem);
            }

            return result;
        }

        public static Dictionary<string, EntityPos> GetAllPlayerPosition(this CommandSender sender)
        {
            PlayerList playerList = GetPlayerList(sender);
            return GetAllEntityPosition(sender, playerList.List);
        }

        public static Dictionary<string, Rotation> GetAllPlayerRotation(this CommandSender sender)
        {
            PlayerList playerList = GetPlayerList(sender);
            return GetAllEntityRotation(sender, playerList.List);
        }

        public static Dictionary<string, Guid> GetAllPlayerUuid(this CommandSender sender)
        {
            PlayerList playerList = GetPlayerList(sender);
            return GetAllEntityUuid(sender, playerList.List);
        }

        public static Dictionary<string, float> GetAllPlayerHealth(this CommandSender sender)
        {
            PlayerList playerList = GetPlayerList(sender);
            return GetAllEntityHealth(sender, playerList.List);
        }

        public static Dictionary<string, Item> GetAllPlayerSelectedItem(this CommandSender sender)
        {
            PlayerList playerList = GetPlayerList(sender);
            return GetAllPlayerSelectedItem(sender, playerList.List);
        }

        public static Dictionary<string, Item> GetAllPlayerDualWieldItem(this CommandSender sender)
        {
            PlayerList playerList = GetPlayerList(sender);
            return GetAllPlayerDualWieldItem(sender, playerList.List);
        }

        #endregion

        public static int SetPlayerHotbarItem(this CommandSender sender, string target, int hotbar, string itemID)
        {
            return ItemReplaceWithEntityHotbarCommand.TrySendCommand(sender, target, hotbar, itemID, out var result) ? result : 0;
        }

        public static bool SendChatMessage(this CommandSender sender, string target, string message, TextColor color = TextColor.White, bool bold = false)
        {
            return TellrawCommand.TrySendCommand(sender, target, CommandUtil.ToJson(message, color, bold));
        }

        public static int SetTitleShowTime(this CommandSender sender, string target, int fadeIn, int stay, int fadeOut)
        {
            return TitleTimesCommand.TrySendCommand(sender, target, fadeIn, stay, fadeOut, out var result) ? result : 0;
        }

        public static int ShowTitle(this CommandSender sender, string target, string message, TextColor color = TextColor.White, bool bold = false)
        {
            return TitleTitleCommand.TrySendCommand(sender, target, CommandUtil.ToJson(message, color, bold), out var result) ? result : 0;
        }

        public static int ShowSubTitle(this CommandSender sender, string target, string message, TextColor color = TextColor.White, bool bold = false)
        {
            return TitleSubTitleCommand.TrySendCommand(sender, target, CommandUtil.ToJson(message, color, bold), out var result) ? result : 0;
        }

        public static int ShowActionbarTitle(this CommandSender sender, string target, string message, TextColor color = TextColor.White, bool bold = false)
        {
            return TitleActionbarCommand.TrySendCommand(sender, target, CommandUtil.ToJson(message, color, bold), out var result) ? result : 0;
        }

        public static int ShowTitle(this CommandSender sender, string target, int fadeIn, int stay, int fadeOut, string message, TextColor color = TextColor.White, bool bold = false)
        {
            SetTitleShowTime(sender, target, fadeIn, stay, fadeOut);
            return ShowTitle(sender, target, message, color, bold);
        }

        public static int ShowSubTitle(this CommandSender sender, string target, int fadeIn, int stay, int fadeOut, string message, TextColor color = TextColor.White, bool bold = false)
        {
            SetTitleShowTime(sender, target, fadeIn, stay, fadeOut);
            return ShowSubTitle(sender, target, message, color, bold);
        }

        public static int ShowActionbarTitle(this CommandSender sender, string target, int fadeIn, int stay, int fadeOut, string message, TextColor color = TextColor.White, bool bold = false)
        {
            SetTitleShowTime(sender, target, fadeIn, stay, fadeOut);
            return ShowActionbarTitle(sender, target, message, color, bold);
        }

        public static int GetPlayerScoreboard(this CommandSender sender, string target, string objective)
        {
            return ScoreboardPlayersGetCommand.TrySendCommand(sender, target, objective, out var result) ? result : 0;
        }

        public static int SetPlayerScoreboard(this CommandSender sender, string target, string objective, int score)
        {
            return ScoreboardPlayersSetCommand.TrySendCommand(sender, target, objective, score, out var result) ? result : 0;
        }

        public static bool AddForceloadChunk(this CommandSender sender, int x, int z)
        {
            return ForceloadAddCommand.TrySendCommand(sender, x, z);
        }

        public static bool AddForceloadChunk<T>(this CommandSender sender, T blockPos) where T : IVector3<int>
        {
            return ForceloadAddCommand.TrySendCommand(sender, blockPos.X, blockPos.Z);
        }

        public static bool RemoveForceloadChunk(this CommandSender sender, int x, int z)
        {
            return ForceloadRemoveCommand.TrySendCommand(sender, x, z);
        }

        public static bool RemoveForceloadChunk<T>(this CommandSender sender, T blockPos) where T : IVector3<int>
        {
            return ForceloadRemoveCommand.TrySendCommand(sender, blockPos.X, blockPos.Z);
        }
    }
}
