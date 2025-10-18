using QuanLib.Game;
using QuanLib.Minecraft.Command.Senders;
using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Models
{
    public class ConditionalRangeCompareCommand(LanguageManager languageManager) :
        ConditionalCommand(TextTemplate.Parse("execute if blocks %s %s %s %s %s %s %s %s %s %s"), languageManager),
        ICreatible<ConditionalRangeCompareCommand>
    {
        public bool TrySendCommand<T>(CommandSender sender, T startPos, T endPos, T destPos, string mode, out int result) where T : IVector3<int>
        {
            ArgumentNullException.ThrowIfNull(startPos, nameof(startPos));
            ArgumentNullException.ThrowIfNull(endPos, nameof(endPos));
            ArgumentNullException.ThrowIfNull(destPos, nameof(destPos));
            ArgumentException.ThrowIfNullOrEmpty(mode, nameof(mode));

            if (base.TrySendCommand(sender, [startPos.X, startPos.Y, startPos.Z, endPos.X, endPos.Y, endPos.Z, destPos.X, destPos.Y, destPos.Z, mode], out var conditionalResult))
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

        public static ConditionalRangeCompareCommand Create(LanguageManager languageManager)
        {
            return new ConditionalRangeCompareCommand(languageManager);
        }
    }
}
