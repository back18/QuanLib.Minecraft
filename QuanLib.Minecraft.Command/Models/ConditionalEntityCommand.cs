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
    public class ConditionalEntityCommand(LanguageManager languageManager) :
        ConditionalCommand(TextTemplate.Parse("execute if entity %s"), languageManager),
        ICreatible<ConditionalEntityCommand>
    {
        public bool TrySendCommand(CommandSender sender, string target, out int result)
        {
            if (base.TrySendCommand(sender, [target], out var conditionalResult))
            {
                result = conditionalResult.Count;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        public static ConditionalEntityCommand Create(LanguageManager languageManager)
        {
            return new ConditionalEntityCommand(languageManager);
        }
    }
}
