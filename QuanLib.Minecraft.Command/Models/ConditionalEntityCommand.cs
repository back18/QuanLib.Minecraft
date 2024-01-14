using QuanLib.Minecraft.Command.Senders;
using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Models
{
    public class ConditionalEntityCommand : ConditionalCountCommandBase
    {
        public ConditionalEntityCommand(LanguageManager languageManager) : base(languageManager)
        {
            Input = TextTemplate.Parse("execute if entity %s");
        }

        public override TextTemplate Input { get; }

        public bool TrySendCommand(CommandSender sender, string target, out int result)
        {
            return base.TrySendCommand(sender, new object[] { target }, out result);
        }
    }
}
