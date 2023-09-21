using QuanLib.Minecraft.Command.Sender;
using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Model
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
            if (string.IsNullOrEmpty(entityID))
                throw new ArgumentException($"“{nameof(entityID)}”不能为 null 或空。", nameof(entityID));
            if (string.IsNullOrEmpty(nbt))
                throw new ArgumentException($"“{nameof(nbt)}”不能为 null 或空。", nameof(nbt));

            return base.TrySendCommand(sender, new object[] { entityID, x, y, z, nbt }, out _);
        }
    }
}
