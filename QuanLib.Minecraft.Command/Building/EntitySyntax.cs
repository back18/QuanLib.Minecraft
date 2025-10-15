using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Building
{
    public class EntitySyntax<TNext> (ICommandSyntax? previous) : CommandSyntax(previous) where TNext : ICreatible<TNext>
    {
        public TNext Player()
        {
            SetSyntax("minecraft:player");
            return TNext.Create(this);
        }

        public TNext Interaction()
        {
            SetSyntax("minecraft:interaction");
            return TNext.Create(this);
        }

        public TNext SetEntity(string entityId)
        {
            ArgumentException.ThrowIfNullOrEmpty(entityId, nameof(entityId));

            SetSyntax(entityId);
            return TNext.Create(this);
        }
    }
}
