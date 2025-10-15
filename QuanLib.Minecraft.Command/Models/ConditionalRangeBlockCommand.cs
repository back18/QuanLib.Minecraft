using QuanLib.Game;
using QuanLib.Minecraft.Command.Senders;
using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Models
{
    public class ConditionalRangeBlockCommand(LanguageManager languageManager) :
        ConditionalCommand(TextTemplate.Parse("execute if blocks %s %s %s %s %s %s %s %s %s masked"), languageManager),
        ICreatible<ConditionalRangeBlockCommand>
    {
        public bool TrySendCommand(CommandSender sender, int startX, int startY, int startZ, int endX, int endY, int endZ, out int result)
        {
            Vector3<int> start = new();
            Vector3<int> end = new();
            start.X = Math.Min(startX, endX);
            start.Y = Math.Min(startY, endY);
            start.Z = Math.Min(startZ, endZ);
            end.X = Math.Max(startX, endX);
            end.Y = Math.Max(startY, endY);
            end.Z = Math.Max(startZ, endZ);

            if (base.TrySendCommand(sender, [start.X, start.Y, start.Z, end.X, end.Y, end.Z, start.X, start.Y, start.Z], out var conditionalResult))
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

        public static ConditionalRangeBlockCommand Create(LanguageManager languageManager)
        {
            return new ConditionalRangeBlockCommand(languageManager);
        }
    }
}
