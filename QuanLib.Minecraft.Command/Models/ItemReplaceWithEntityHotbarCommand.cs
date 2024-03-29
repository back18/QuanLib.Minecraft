﻿using QuanLib.Minecraft.Command.Senders;
using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Models
{
    public class ItemReplaceWithEntityHotbarCommand : MultipleCommandBase
    {
        public ItemReplaceWithEntityHotbarCommand(LanguageManager languageManager)
        {
            ArgumentNullException.ThrowIfNull(languageManager, nameof(languageManager));

            MultipleOutput = languageManager["commands.item.entity.set.success.multiple"];
            Output = languageManager["commands.item.entity.set.success.single"];
            Input = TextTemplate.Parse("item replace entity %s hotbar.%s with %s");
        }

        public override TextTemplate Input { get; }

        public override TextTemplate Output { get; }

        public override TextTemplate MultipleOutput { get; }

        public bool TrySendCommand(CommandSender sender, string target, int hotbar, string itemID, out int result)
        {
            ArgumentException.ThrowIfNullOrEmpty(target, nameof(target));
            ArgumentException.ThrowIfNullOrEmpty(itemID, nameof(itemID));

            itemID = itemID.Replace("\\", "\\\\");
            return base.TrySendCommand(sender, new object[] { target, hotbar, itemID }, out result);
        }

        public override bool TryParseResult(string[] outargs, [MaybeNullWhen(false)] out int result)
        {
            return base.TryParseResult(outargs, 2, 0, out result);
        }
    }
}
