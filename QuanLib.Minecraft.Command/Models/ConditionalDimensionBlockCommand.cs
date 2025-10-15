using QuanLib.Minecraft.Command.Senders;
using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Models
{
    public class ConditionalDimensionBlockCommand(LanguageManager languageManager) :
        ConditionalCommand(TextTemplate.Parse("execute in %s if block %s %s %s %s"), languageManager),
        ICreatible<ConditionalDimensionBlockCommand>
    {
        public bool TrySendCommand(CommandSender sender, string dimension, int x, int y, int z, string blockId, out bool result)
        {
            ArgumentException.ThrowIfNullOrEmpty(dimension, nameof(dimension));
            ArgumentException.ThrowIfNullOrEmpty(blockId, nameof(blockId));

            if (base.TrySendCommand(sender, [dimension, x, y, z, blockId], out var conditionalResult))
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

        public static ConditionalDimensionBlockCommand Create(LanguageManager languageManager)
        {
            return new ConditionalDimensionBlockCommand(languageManager);
        }
    }
}
