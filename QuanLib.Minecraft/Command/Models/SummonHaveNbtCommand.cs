using QuanLib.Minecraft.Command.Senders;
using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Models
{
    public class SummonHaveNbtCommand : SummonCommandBase
    {
        public SummonHaveNbtCommand()
        {
            Input = TextTemplate.Parse("summon %s %s %s %s %s");
        }

        public override TextTemplate Input { get; }

        public bool TrySendCommand(CommandSender sender, double x, double y, double z, string entityID, string nbt)
        {
            ArgumentException.ThrowIfNullOrEmpty(entityID, nameof(entityID));
            ArgumentException.ThrowIfNullOrEmpty(nbt, nameof(nbt));

            return base.TrySendCommand(sender, new object[] { entityID, x, y, z, nbt }, out _);
        }
    }
}
