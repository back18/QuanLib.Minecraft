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
using QuanLib.Minecraft.ResourcePack.Language;
using QuanLib.Game;
using System.ComponentModel;

namespace QuanLib.Minecraft.Command
{
    public static class CommandManager
    {
        private const string AIR_BLOCK = "minecraft:air";

        static CommandManager()
        {
            _languageManager = LanguageManager.Instance;

            ListCommand = new(_languageManager);
            TimeQueryDayCommand = new(_languageManager);
            TimeQueryDaytimeCommand = new(_languageManager);
            TimeQueryGametimeCommand = new(_languageManager);
            SetBlockCommand = new(_languageManager);
            FillCommand = new(_languageManager);
            SummonCommand = new(_languageManager);
            SummonHaveNbtCommand = new(_languageManager);
            KillCommand = new(_languageManager);
            TelePortEntityCommand = new(_languageManager);
            TelePortLocationCommand = new(_languageManager);
            ConditionalBlockCommand = new(_languageManager);
            ConditionalDimensionBlockCommand = new(_languageManager);
            ConditionalRangeBlockCommand = new(_languageManager);
            ConditionalRangeCompareCommand = new(_languageManager);
            ConditionalEntityCommand = new(_languageManager);
            DataGetEntityCommand = new(_languageManager);
            DataGetEntityHavePathCommand = new(_languageManager);
            ItemReplaceWithEntityHotbarCommand = new(_languageManager);
            TellrawCommand = new();
            TitleTitleCommand = new(_languageManager);
            TitleSubTitleCommand = new(_languageManager);
            TitleActionbarCommand = new(_languageManager);
            TitleTimesCommand = new(_languageManager);
            ScoreboardPlayersGetCommand = new(_languageManager);
            ScoreboardPlayersSetCommand = new(_languageManager);
            ForceloadAddCommand = new(_languageManager);
            ForceloadRemoveCommand = new(_languageManager);
        }

        private static readonly LanguageManager _languageManager;

        public static readonly ListCommand ListCommand;
        public static readonly TimeQueryDayCommand TimeQueryDayCommand;
        public static readonly TimeQueryDaytimeCommand TimeQueryDaytimeCommand;
        public static readonly TimeQueryGametimeCommand TimeQueryGametimeCommand;
        public static readonly SetBlockCommand SetBlockCommand;
        public static readonly FillCommand FillCommand;
        public static readonly SummonCommand SummonCommand;
        public static readonly SummonHaveNbtCommand SummonHaveNbtCommand;
        public static readonly KillCommand KillCommand;
        public static readonly TelePortEntityCommand TelePortEntityCommand;
        public static readonly TelePortLocationCommand TelePortLocationCommand;
        public static readonly ConditionalBlockCommand ConditionalBlockCommand;
        public static readonly ConditionalDimensionBlockCommand ConditionalDimensionBlockCommand;
        public static readonly ConditionalRangeBlockCommand ConditionalRangeBlockCommand;
        public static readonly ConditionalRangeCompareCommand ConditionalRangeCompareCommand;
        public static readonly ConditionalEntityCommand ConditionalEntityCommand;
        public static readonly DataGetEntityCommand DataGetEntityCommand;
        public static readonly DataGetEntityHavePathCommand DataGetEntityHavePathCommand;
        public static readonly ItemReplaceWithEntityHotbarCommand ItemReplaceWithEntityHotbarCommand;
        public static readonly TellrawCommand TellrawCommand;
        public static readonly TitleTitleCommand TitleTitleCommand;
        public static readonly TitleSubTitleCommand TitleSubTitleCommand;
        public static readonly TitleActionbarCommand TitleActionbarCommand;
        public static readonly TitleTimesCommand TitleTimesCommand;
        public static readonly ScoreboardPlayersGetCommand ScoreboardPlayersGetCommand;
        public static readonly ScoreboardPlayersSetCommand ScoreboardPlayersSetCommand;
        public static readonly ForceloadAddCommand ForceloadAddCommand;
        public static readonly ForceloadRemoveCommand ForceloadRemoveCommand;

        public static SnbtCache SnbtCache { get; } = new(TimeSpan.FromMilliseconds(50));

        public static T CreateCommand<T>() where T : ICreatible<T>
        {
            return T.Create(_languageManager);
        }

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

        public static bool SetBlock(this CommandSender sender, int x, int y, int z, string blockId)
        {
            return SetBlockCommand.TrySendCommand(sender, x, y, z, blockId);
        }

        public static bool SetBlock<T>(this CommandSender sender, T position, string blockId) where T : IVector3<int>
        {
            ArgumentNullException.ThrowIfNull(position, nameof(position));

            return SetBlockCommand.TrySendCommand(sender, position.X, position.Y, position.Z, blockId);
        }

        public static int Fill(this CommandSender sender, int x1, int y1, int z1, int x2, int y2, int z2, string blockId, bool split = false, int maxBlocks = 32768)
        {
            if (!split)
                return Execute(x1, y1, z1, x2, y2, z2);

            int successCount = 0;
            CubeRange[] cubeRanges = SplitRange(x1, y1, z1, x2, y2, z2, maxBlocks);

            foreach (CubeRange cubeRange in cubeRanges)
                successCount += Execute(
                    cubeRange.StartPosition.X,
                    cubeRange.StartPosition.Y,
                    cubeRange.StartPosition.Z,
                    cubeRange.EndPosition.X,
                    cubeRange.EndPosition.Y,
                    cubeRange.EndPosition.Z);

            return successCount;

            int Execute(int x1, int y1, int z1, int x2, int y2, int z2)
            {
                return FillCommand.TrySendCommand(sender, x1, y1, z1, x2, y2, z2, blockId, out var result) ? result : 0;
            }
        }

        public static int Fill<T>(this CommandSender sender, T startPos, T endPos, string blockId, bool split = false, int maxBlocks = 32768) where T : IVector3<int>
        {
            return Fill(sender, startPos.X, startPos.Y, startPos.Z, endPos.X, endPos.Y, endPos.Z, blockId, split, maxBlocks);
        }

        public static bool SummonEntity(this CommandSender sender, double x, double y, double z, string entityId, string? nbt = null)
        {
            if (string.IsNullOrEmpty(nbt))
                return SummonCommand.TrySendCommand(sender, x, y, z, entityId);
            else
                return SummonHaveNbtCommand.TrySendCommand(sender, x, y, z, entityId, nbt);
        }

        public static bool SummonEntity<T>(this CommandSender sender, T position, string entityId, string? nbt = null) where T : IVector3<double>
        {
            ArgumentNullException.ThrowIfNull(position, nameof(position));

            if (string.IsNullOrEmpty(nbt))
                return SummonCommand.TrySendCommand(sender, position.X, position.Y, position.Z, entityId);
            else
                return SummonHaveNbtCommand.TrySendCommand(sender, position.X, position.Y, position.Z, entityId, nbt);
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

        public static bool CheckBlock(this CommandSender sender, int x, int y, int z, string blockId)
        {
            return ConditionalBlockCommand.TrySendCommand(sender, x, y, z, blockId, out var result) ? result : false;
        }

        public static bool CheckBlock<T>(this CommandSender sender, T position, string blockId) where T : IVector3<int>
        {
            ArgumentNullException.ThrowIfNull(position, nameof(position));

            return ConditionalBlockCommand.TrySendCommand(sender, position.X, position.Y, position.Z, blockId, out var result) ? result : false;
        }

        public static bool CheckBlock(this CommandSender sender, string dimension, int x, int y, int z, string blockId)
        {
            return ConditionalDimensionBlockCommand.TrySendCommand(sender, dimension, x, y, z, blockId, out var result) ? result : false;
        }

        public static bool CheckBlock<T>(this CommandSender sender, string dimension, T position, string blockId) where T : IVector3<int>
        {
            ArgumentNullException.ThrowIfNull(position, nameof(position));

            return ConditionalDimensionBlockCommand.TrySendCommand(sender, dimension, position.X, position.Y, position.Z, blockId, out var result) ? result : false;
        }

        public static bool CheckRangeBlock(this CommandSender sender, int x1, int y1, int z1, int x2, int y2, int z2, string blockId, int maxBlocks = 32768)
        {
            ArgumentException.ThrowIfNullOrEmpty(blockId, nameof(blockId));

            if (blockId == AIR_BLOCK)
                return GetBlockCount(sender, x1, y1, z1, x2, y2, z2, true) == 0;

            int startX = Math.Min(x1, x2);
            int startY = Math.Min(y1, y2);
            int startZ = Math.Min(z1, z2);
            int endX = Math.Max(x1, x2);
            int endY = Math.Max(y1, y2);
            int endZ = Math.Max(z1, z2);
            int xLength = endX - startX + 1;
            int yLength = endY - startY + 1;
            int zLength = endZ - startZ + 1;
            int totalBlocks = xLength * yLength * zLength;
            int minLength = new int[] { xLength, yLength, zLength }.Min();

            if (xLength == minLength)
            {
                for (int i = 0; i < xLength; i++)
                {
                    if (!CheckBlock(sender, i, startY, startZ, blockId))
                        return false;
                }

                CubeRange sourceRange = new(startX, startY, startZ, endX, startY, startZ);
                if (yLength < zLength)
                {
                    if (!CheckY(sourceRange))
                        return false;
                    sourceRange = new(startX, startY, startZ, endX, endY, startZ);
                    return CheckZ(sourceRange);
                }
                else
                {
                    if (!CheckZ(sourceRange))
                        return false;
                    sourceRange = new(startX, startY, startZ, endX, startY, endZ);
                    return CheckY(sourceRange);
                }
            }
            else if (yLength == minLength)
            {
                for (int i = 0; i < yLength; i++)
                {
                    if (!CheckBlock(sender, startX, i, startZ, blockId))
                        return false;
                }

                CubeRange sourceRange = new(startX, startY, startZ, startX, endY, startZ);
                if (xLength < zLength)
                {
                    if (!CheckX(sourceRange))
                        return false;
                    sourceRange = new(startX, startY, startZ, endX, endY, startZ);
                    return CheckZ(sourceRange);
                }
                else
                {
                    if (!CheckZ(sourceRange))
                        return false;
                    sourceRange = new(startX, startY, startZ, startX, endY, endZ);
                    return CheckX(sourceRange);
                }
            }
            else
            {
                for (int i = 0; i < zLength; i++)
                {
                    if (!CheckBlock(sender, startX, startY, i, blockId))
                        return false;
                }

                CubeRange sourceRange = new(startX, startY, startZ, startX, startY, endZ);
                if (xLength < yLength)
                {
                    if (!CheckX(sourceRange))
                        return false;
                    sourceRange = new(startX, startY, startZ, endX, startY, endZ);
                    return CheckY(sourceRange);
                }
                else
                {
                    if (!CheckY(sourceRange))
                        return false;
                    sourceRange = new(startX, startY, startZ, startX, endY, endZ);
                    return CheckX(sourceRange);
                }
            }

            bool CheckX(CubeRange sourceRange)
            {
                while (true)
                {
                    CubeRange destRange = CloneRange(sourceRange, Facing.Xp);
                    if (!Execute(sourceRange.StartPosition, sourceRange.EndPosition, destRange.StartPosition))
                        return false;

                    if (destRange.EndPosition.X >= endX)
                        return true;

                    sourceRange = new(sourceRange.StartPosition, destRange.EndPosition);

                    if (sourceRange.Volume > maxBlocks)
                    {
                        int area = totalBlocks / xLength ;
                        if (area > maxBlocks)
                            throw new InvalidOperationException("区域过大，无法拆分");

                        int step = maxBlocks / area;
                        int offset = sourceRange.Range.X - step;
                        sourceRange.StartPosition = sourceRange.StartPosition.Offset(Facing.Xp, offset);
                    }

                    int overflowLength = sourceRange.Range.X * 2 - xLength;
                    if (overflowLength > 0)
                        sourceRange.StartPosition = sourceRange.StartPosition.Offset(Facing.Xp, overflowLength);
                }
            }

            bool CheckY(CubeRange sourceRange)
            {
                while (true)
                {
                    CubeRange destRange = CloneRange(sourceRange, Facing.Yp);
                    if (!Execute(sourceRange.StartPosition, sourceRange.EndPosition, destRange.StartPosition))
                        return false;

                    if (destRange.EndPosition.Y >= endY)
                        return true;

                    sourceRange = new(sourceRange.StartPosition, destRange.EndPosition);

                    if (sourceRange.Volume > maxBlocks)
                    {
                        int area = totalBlocks / yLength;
                        if (area > maxBlocks)
                            throw new InvalidOperationException("区域过大，无法拆分");

                        int step = maxBlocks / area;
                        int offset = sourceRange.Range.Y - step;
                        sourceRange.StartPosition = sourceRange.StartPosition.Offset(Facing.Yp, offset);
                    }

                    int overflowLength = sourceRange.Range.Y * 2 - yLength;
                    if (overflowLength > 0)
                        sourceRange.StartPosition = sourceRange.StartPosition.Offset(Facing.Yp, overflowLength);
                }
            }

            bool CheckZ(CubeRange sourceRange)
            {
                while (true)
                {
                    CubeRange destRange = CloneRange(sourceRange, Facing.Zp);
                    if (!Execute(sourceRange.StartPosition, sourceRange.EndPosition, destRange.StartPosition))
                        return false;

                    if (destRange.EndPosition.Z >= endZ)
                        return true;

                    sourceRange = new(sourceRange.StartPosition, destRange.EndPosition);

                    if (sourceRange.Volume > maxBlocks)
                    {
                        int area = totalBlocks / zLength;
                        if (area > maxBlocks)
                            throw new InvalidOperationException("区域过大，无法拆分");

                        int step = maxBlocks / area;
                        int offset = sourceRange.Range.Z - step;
                        sourceRange.StartPosition = sourceRange.StartPosition.Offset(Facing.Zp, offset);
                    }

                    int overflowLength = sourceRange.Range.Z * 2 - zLength;
                    if (overflowLength > 0)
                        sourceRange.StartPosition = sourceRange.StartPosition.Offset(Facing.Zp, overflowLength);
                }
            }

            bool Execute(Vector3<int> startPos, Vector3<int> endPos, Vector3<int> destPos)
            {
                return ConditionalRangeCompareCommand.TrySendCommand(sender, startPos, endPos, destPos, "all", out var result) ? result > 0 : false;
            }
        }

        public static bool CheckRangeBlock<T>(this CommandSender sender, T startPos, T endPos, string blockId, int maxBlocks = 32768) where T : IVector3<int>
        {
            return CheckRangeBlock(sender, startPos.X, startPos.Y, startPos.Z, endPos.X, endPos.Y, endPos.Z, blockId, maxBlocks);
        }

        public static int GetBlockCount(this CommandSender sender, int startX, int startY, int startZ, int endX, int endY, int endZ, bool split = false, int maxBlocks = 32768)
        {
            if (!split)
                return Execute(startX, startY, startZ, endX, endY, endZ);

            int successCount = 0;
            CubeRange[] cubeRanges = SplitRange(startX, startY, startZ, endX, endY, endZ, maxBlocks);

            foreach (CubeRange cubeRange in cubeRanges)
                successCount += Execute(
                    cubeRange.StartPosition.X,
                    cubeRange.StartPosition.Y,
                    cubeRange.StartPosition.Z,
                    cubeRange.EndPosition.X,
                    cubeRange.EndPosition.Y,
                    cubeRange.EndPosition.Z);

            return successCount;

            int Execute(int x1, int y1, int z1, int x2, int y2, int z2)
            {
                return ConditionalRangeBlockCommand.TrySendCommand(sender, x1, y1, z1, x2, y2, z2, out var result) ? result : 0;
            }
        }

        public static int GetBlockCount<T>(this CommandSender sender, T startPos, T endPos, bool split = false, int maxBlocks = 32768) where T : IVector3<int>
        {
            ArgumentNullException.ThrowIfNull(startPos, nameof(startPos));
            ArgumentNullException.ThrowIfNull(endPos, nameof(endPos));

            return GetBlockCount(sender, startPos.X, startPos.Y, startPos.Z, endPos.X, endPos.Y, endPos.Z, split, maxBlocks);
        }

        public static bool CheckEntity(this CommandSender sender, string target)
        {
            return GetEntityCount(sender, target) > 0;
        }

        public static int GetEntityCount(this CommandSender sender, string target)
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

        public static bool TryGetEntityPosition(this CommandSender sender, string target, out Vector3<double> result)
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
            }
        }

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

        public static Dictionary<string, Vector3<double>> GetAllEntityPosition(this CommandSender sender, IEnumerable<string> targets)
        {
            ArgumentNullException.ThrowIfNull(targets, nameof(targets));

            Dictionary<string, Vector3<double>> result = new();
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

        public static Dictionary<string, Vector3<double>> GetAllPlayerPosition(this CommandSender sender)
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

        public static int SetPlayerHotbarItem(this CommandSender sender, string target, int hotbar, string itemId)
        {
            return ItemReplaceWithEntityHotbarCommand.TrySendCommand(sender, target, hotbar, itemId, out var result) ? result : 0;
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

        private static CubeRange[] SplitRange(int x1, int y1, int z1, int x2, int y2, int z2, int maxBlocks)
        {
            int startX = Math.Min(x1, x2);
            int startY = Math.Min(y1, y2);
            int startZ = Math.Min(z1, z2);
            int endX = Math.Max(x1, x2);
            int endY = Math.Max(y1, y2);
            int endZ = Math.Max(z1, z2);
            int xLength = endX - startX + 1;
            int yLength = endY - startY + 1;
            int zLength = endZ - startZ + 1;
            int totalBlocks = xLength * yLength * zLength;

            if (totalBlocks <= maxBlocks)
                return [new CubeRange(x1, y1, z1, x2, y2, z2)];

            int maxLength = new int[] { xLength, yLength, zLength }.Max();
            int area = totalBlocks / maxLength;

            if (area > maxBlocks)
                throw new InvalidOperationException("区域过大，无法拆分");

            int step = maxBlocks / area;
            List<CubeRange> cubeRanges = [];

            if (maxLength == xLength)
            {
                for (int sx = startX; sx <= endX; sx += step)
                {
                    int ex = Math.Min(sx + step - 1, endX);
                    cubeRanges.Add(new CubeRange(sx, startY, startZ, ex, endY, endZ));
                }
            }
            else if (maxLength == yLength)
            {
                for (int sy = startY; sy <= endY; sy += step)
                {
                    int ey = Math.Min(sy + step - 1, endY);
                    cubeRanges.Add(new CubeRange(startX, sy, startZ, endX, ey, endZ));
                }
            }
            else
            {
                for (int sz = startZ; sz <= endZ; sz += step)
                {
                    int ez = Math.Min(sz + step - 1, endZ);
                    cubeRanges.Add(new CubeRange(startX, startY, sz, endX, endY, ez));
                }
            }

            return cubeRanges.ToArray();
        }

        private static CubeRange CloneRange(CubeRange cubeRange, Facing facing)
        {
            int x1 = cubeRange.StartPosition.X;
            int y1 = cubeRange.StartPosition.Y;
            int z1 = cubeRange.StartPosition.Z;
            int x2 = cubeRange.EndPosition.X;
            int y2 = cubeRange.EndPosition.Y;
            int z2 = cubeRange.EndPosition.Z;
            int length;

            switch (facing)
            {
                case Facing.Xp:
                    length = Math.Abs(x2 - x1) + 1;
                    x1 += length;
                    x2 += length;
                    break;
                case Facing.Xm:
                    length = Math.Abs(x2 - x1) + 1;
                    x1 -= length;
                    x2 -= length;
                    break;
                case Facing.Yp:
                    length = Math.Abs(y2 - y1) + 1;
                    y1 += length;
                    y2 += length;
                    break;
                case Facing.Ym:
                    length = Math.Abs(y2 - y1) + 1;
                    y1 -= length;
                    y2 -= length;
                    break;
                case Facing.Zp:
                    length = Math.Abs(z2 - z1) + 1;
                    z1 += length;
                    z2 += length;
                    break;
                case Facing.Zm:
                    length = Math.Abs(z2 - z1) + 1;
                    z1 -= length;
                    z2 -= length;
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(facing), (int)facing, typeof(Facing));
            }

            return new(x1, y1, z1, x2, y2, z2);
        }

        public static ConditionalCommand Conditional(this Building.ExecuteCommandSyntax source)
        {
            string command = source.Build();
            return new ConditionalCommand(TextTemplate.Parse(command), _languageManager);
        }
    }
}
