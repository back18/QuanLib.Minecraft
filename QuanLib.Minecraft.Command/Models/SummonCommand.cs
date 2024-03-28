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
        public SummonCommand(LanguageManager languageManager) : base(languageManager)
        {
            Input = TextTemplate.Parse("summon %s %s %s %s");
        }

        public override TextTemplate Input { get; }

        public bool TrySendCommand(CommandSender sender, double x, double y, double z, string entityId)
        {
            ArgumentException.ThrowIfNullOrEmpty(entityId, nameof(entityId));

            return base.TrySendCommand(sender, new object[] { entityId, x, y, z }, out _);
        }
    }
}
