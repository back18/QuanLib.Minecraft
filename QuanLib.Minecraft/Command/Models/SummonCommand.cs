using QuanLib.Minecraft.Command.Senders;
using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Models
{
    public class SummonCommand : SummonCommandBase
    {
        public SummonCommand()
        {
            Input = TextTemplate.Parse("summon %s %s %s %s");
        }

        public override TextTemplate Input { get; }

        public bool TrySendCommand(CommandSender sender, double x, double y, double z, string entityID)
        {
            if (string.IsNullOrEmpty(entityID))
                throw new ArgumentException($"“{nameof(entityID)}”不能为 null 或空。", nameof(entityID));

            return base.TrySendCommand(sender, new object[] { entityID, x, y, z }, out _);
        }
    }
}
