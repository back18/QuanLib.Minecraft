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
    public class ItemReplaceWithEntityHotbarCommand : CountCommandBase
    {
        public ItemReplaceWithEntityHotbarCommand()
        {
            Input = TextTemplate.Parse("item replace entity %s hotbar.%s with %s");
            Output = LanguageManager.Instance["commands.item.entity.set.success.single"];
            CountOutput = LanguageManager.Instance["commands.item.entity.set.success.multiple"];
        }

        public override TextTemplate Input { get; }

        public override TextTemplate Output { get; }

        public override TextTemplate CountOutput { get; }

        public bool TrySendCommand(CommandSender sender, string target, int hotbar, string itemID, out int result)
        {
            if (string.IsNullOrEmpty(target))
                throw new ArgumentException($"“{nameof(target)}”不能为 null 或空。", nameof(target));
            if (string.IsNullOrEmpty(itemID))
                throw new ArgumentException($"“{nameof(itemID)}”不能为 null 或空。", nameof(itemID));

            itemID = itemID.Replace("\\", "\\\\");
            return base.TrySendCommand(sender, new object[] { target, hotbar, itemID }, out result);
        }

        public override bool TryParseResult(string[] outargs, [MaybeNullWhen(false)] out int result)
        {
            return base.TryParseResult(outargs, 2, 0, out result);
        }
    }
}
