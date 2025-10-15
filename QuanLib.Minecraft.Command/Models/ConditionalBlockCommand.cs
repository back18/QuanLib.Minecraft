using QuanLib.Minecraft.Command.Senders;
using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Models
{
    public class ConditionalBlockCommand(LanguageManager languageManager) :
        ConditionalCommand(TextTemplate.Parse("execute if block %s %s %s %s"),
        languageManager), ICreatible<ConditionalBlockCommand>
    {
        public bool TrySendCommand(CommandSender sender, int x, int y, int z, string blockId, out bool result)
        {
            ArgumentException.ThrowIfNullOrEmpty(blockId, nameof(blockId));

            if (base.TrySendCommand(sender, [x, y, z, blockId], out var conditionalResult))
            {
                result = conditionalResult.IsSuccess;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }

        public static ConditionalBlockCommand Create(LanguageManager languageManager)
        {
            return new ConditionalBlockCommand(languageManager);
        }
    }
}
