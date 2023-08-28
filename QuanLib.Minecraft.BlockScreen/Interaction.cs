using QuanLib.Minecraft.Selectors;
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
        private const string INTERACTION_ENTITY = "minecraft:interaction";

        private const string INTERACTION_NBT = "{width:3,height:3,response:true}";

        public Interaction(string player, string uUID, EntityPos position)
        {
            Player = player;
            UUID = uUID;
            Position = position;
        }

        public string Player { get; }

        public string UUID { get; }

        public EntityPos Position { get; private set; }

        public bool SyncPosition()
        {
            var command = MCOS.Instance.MinecraftServer.CommandHelper;
            if (!command.TryGetEntityPosition(Player, out var position))
                return false;

            Position = position;
            return command.TelePort(new GenericSelector(UUID), new GenericSelector(Player));
        }

        public static bool TryCreate(string player, [MaybeNullWhen(false)] out Interaction result)
        {
            if (string.IsNullOrEmpty(player))
                goto err;

            var command = MCOS.Instance.MinecraftServer.CommandHelper;
            if (!command.TryGetEntityPosition(player, out var position))
                goto err;

            if (!command.Summon(INTERACTION_ENTITY, position, INTERACTION_NBT))
                goto err;



            err:
            result = null;
            return false;
        }
    }
}
