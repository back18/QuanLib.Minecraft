using Newtonsoft.Json;
using QuanLib.Minecraft.Selectors;
using QuanLib.Minecraft.Snbt.Data;
using QuanLib.Minecraft.Vector;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public class Interaction
    {
        private const string INTERACTION_ID = "minecraft:interaction";

        private const string INTERACTION_NBT = "{width:3,height:3,response:true}";

        private Interaction(string playerName, Guid playerUUID, Guid entityUUID, EntityPos position)
        {
            PlayerName = playerName;
            PlayerUUID = playerUUID;
            EntityUUID = entityUUID;
            Position = position;
            InteractionState = InteractionState.Active;

            _player = PlayerUUID.ToString();
            _entity = EntityUUID.ToString();

            _ = SaveJsonAsync();
        }

        private readonly string _player;

        private readonly string _entity;

        private Task? _task;

        public InteractionState InteractionState { get; private set; }

        public string PlayerName { get; }

        public Guid PlayerUUID { get; }

        public Guid EntityUUID { get; }

        public EntityPos Position { get; private set; }

        public long LeftClickTimestamp { get; private set; }

        public long RightClickTimestamp { get; private set; }

        public bool IsLeftClick { get; private set; }

        public bool IsRightClick { get; private set; }

        public void Handle()
        {
            Test();
            switch (InteractionState)
            {
                case InteractionState.Active:
                    ReadLeftRightKeys();
                    SyncPosition();
                    _task?.Wait();
                    _ = SaveJsonAsync();
                    break;
                case InteractionState.Offline:
                    Close();
                    break;
                case InteractionState.Closed:
                    break;
                default:
                    break;
            }
        }

        public bool Test()
        {
            var command = MCOS.Instance.MinecraftServer.CommandHelper;
            bool result = command.TestEntity(new GenericSelector(_player)) && command.TestEntity(new GenericSelector(_entity));
            if (!result && InteractionState == InteractionState.Active)
                InteractionState = InteractionState.Offline;
            return result;
        }

        public bool SyncPosition()
        {
            var command = MCOS.Instance.MinecraftServer.CommandHelper;
            if (!command.TryGetEntityPosition(_player, out var position))
                return false;

            Position = position;
            return command.TelePort(new GenericSelector(_entity), new GenericSelector(_player));
        }

        public void ReadLeftRightKeys()
        {
            IsLeftClick = false;
            IsRightClick = false;
            var command = MCOS.Instance.MinecraftServer.CommandHelper;
            if (!command.TryGetInteractionData(_entity, out var keys))
                return;

            if (keys.LeftClick.Timestamp > LeftClickTimestamp)
            {
                LeftClickTimestamp = keys.LeftClick.Timestamp;
                if (keys.LeftClick.Player == PlayerUUID)
                    IsLeftClick = true;
            }
            if (keys.RightClick.Timestamp > RightClickTimestamp)
            {
                RightClickTimestamp = keys.RightClick.Timestamp;
                if (keys?.RightClick.Player == PlayerUUID)
                    IsRightClick = true;
            }
        }

        public void Close()
        {
            var command = MCOS.Instance.MinecraftServer.CommandHelper;
            BlockPos blockPos = Position.ToBlockPos();
            command.AddForceLoadChunk(blockPos);
            command.KillEntity(new GenericSelector(_entity));
            command.RemoveForceLoadChunk(blockPos);
            DaleteJson();
            InteractionState = InteractionState.Closed;
        }

        public async Task SaveJsonAsync()
        {
            _task?.Wait();
            _task = File.WriteAllTextAsync(MCOS.MainDirectory.Saves.Interactions.Combine(_player + ".json"), JsonConvert.SerializeObject(ToJson()));
            await _task;
        }

        public void DaleteJson()
        {
            string path = MCOS.MainDirectory.Saves.Interactions.Combine(_player + ".json");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public Json ToJson()
        {
            return new(_player, _entity, new double[] { Position.X, Position.Y, Position.Z });
        }

        public static bool TryCreate(string player, [MaybeNullWhen(false)] out Interaction result)
        {
            if (string.IsNullOrEmpty(player))
                goto err;

            var command = MCOS.Instance.MinecraftServer.CommandHelper;

            if (!command.TryGetEntityUUID(player, out var playerUUID))
                goto err;

            if (!command.TryGetEntityPosition(player, out var position))
                goto err;

            if (command.TestEntity(new GenericSelector($"@e[type=minecraft:interaction,x={position.X},y={position.Y},z={position.Z},distance=..1]")))
                goto err;

            if (!command.SummonEntity(INTERACTION_ID, position, INTERACTION_NBT))
                goto err;

            string snbt = command.GetAllEntitySbnt($"@e[type=minecraft:interaction,x={position.X},y={position.Y},z={position.Z},distance=..0.1]", "UUID");
            if (!MinecraftUtil.TryParseUUIDSbnt(snbt, out var entityUUID))
                goto err;

            result = new(player, playerUUID, entityUUID, position);
            return true;

            err:
            result = null;
            return false;
        }

        public class Json
        {
            public Json(string playerUUID, string entityUUID, double[] position)
            {
                if (string.IsNullOrEmpty(playerUUID))
                    throw new ArgumentException($"“{nameof(playerUUID)}”不能为 null 或空。", nameof(playerUUID));
                if (string.IsNullOrEmpty(entityUUID))
                    throw new ArgumentException($"“{nameof(entityUUID)}”不能为 null 或空。", nameof(entityUUID));
                if (position is null)
                    throw new ArgumentNullException(nameof(position));

                PlayerUUID = playerUUID;
                EntityUUID = entityUUID;
                Position = position;
            }

            public string PlayerUUID { get; set; }

            public string EntityUUID { get; set; }

            public double[] Position { get; set; }
        }
    }
}
