using Newtonsoft.Json;
using QuanLib.Minecraft.Command;
using QuanLib.Minecraft.Command.Sender;
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
    public class Interaction : IDisposable
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
            _file = Path.Combine(_dir, _player + ".json");

            _task = SaveJsonAsync();
        }

        private readonly string _player;

        private readonly string _entity;

        private static readonly string _dir = MCOS.MainDirectory.Saves.Interactions.FullPath;

        private readonly string _file;

        private Task _task;

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
            ConditionalEntity();
            switch (InteractionState)
            {
                case InteractionState.Active:
                    ReadLeftRightKeys();
                    SyncPosition();
                    _task = SaveJsonAsync();
                    break;
                case InteractionState.Offline:
                    Dispose();
                    break;
                case InteractionState.Closed:
                    break;
                default:
                    break;
            }
        }

        public bool ConditionalEntity()
        {
            CommandSender sender = MCOS.Instance.MinecraftInstance.CommandSender;
            bool result = sender.ConditionalEntity(_player) && sender.ConditionalEntity(_entity);
            if (!result && InteractionState == InteractionState.Active)
                InteractionState = InteractionState.Offline;
            return result;
        }

        public bool SyncPosition()
        {
            CommandSender sender = MCOS.Instance.MinecraftInstance.CommandSender;
            if (!sender.TryGetEntityPosition(_player, out var position))
                return false;

            Position = position;
            return sender.TelePort(_entity, Position) > 0;
        }

        public void ReadLeftRightKeys()
        {
            IsLeftClick = false;
            IsRightClick = false;
            CommandSender sender = MCOS.Instance.MinecraftInstance.CommandSender;
            LeftRightKeys keys = sender.GetInteractionData(_entity);

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

        private async Task SaveJsonAsync()
        {
            _task?.Wait();
            if (!Directory.Exists(_dir))
                Directory.CreateDirectory(_dir);
            await File.WriteAllTextAsync(_file, JsonConvert.SerializeObject(ToJson()));
        }

        private void DaleteJson()
        {
            if (File.Exists(_file))
                File.Delete(_file);
        }

        private Json ToJson()
        {
            return new(_player, _entity, new double[] { Position.X, Position.Y, Position.Z });
        }

        public void Dispose()
        {
            CommandSender sender = MCOS.Instance.MinecraftInstance.CommandSender;
            BlockPos blockPos = Position.ToBlockPos();
            sender.AddForceloadChunk(blockPos);
            sender.KillEntity(_entity);
            sender.RemoveForceloadChunk(blockPos);
            DaleteJson();
            InteractionState = InteractionState.Closed;
            GC.SuppressFinalize(this);
        }

        public static bool TryCreate(string player, [MaybeNullWhen(false)] out Interaction result)
        {
            if (string.IsNullOrEmpty(player))
                goto fail;

            CommandSender sender = MCOS.Instance.MinecraftInstance.CommandSender;

            if (!sender.TryGetEntityUuid(player, out var playerUUID))
                goto fail;

            if (!sender.TryGetEntityPosition(player, out var position))
                goto fail;

            if (sender.ConditionalEntity($"@e[limit=1,type=minecraft:interaction,x={position.X},y={position.Y},z={position.Z},distance=..1,sort=nearest]"))
                goto fail;

            if (!sender.SummonEntity(position, INTERACTION_ID, INTERACTION_NBT))
                goto fail;

            if (!sender.TryGetEntityUuid($"@e[limit=1,type=minecraft:interaction,x={position.X},y={position.Y},z={position.Z},distance=..1,sort=nearest]", out var entityUUID))
                goto fail;

            result = new(player, playerUUID, entityUUID, position);
            return true;

            fail:
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
