﻿using QuanLib.Minecraft.Command.Senders;
using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Models
{
    public class ConditionalBlockCommand : ConditionalCommandBase
    {
        public ConditionalBlockCommand(LanguageManager languageManager) : base(languageManager)
        {
            Input = TextTemplate.Parse("execute if block %s %s %s %s");
        }

        public override TextTemplate Input { get; }

        public bool TrySendCommand(CommandSender sender, int x, int y, int z, string blockId)
        {
            ArgumentException.ThrowIfNullOrEmpty(blockId, nameof(blockId));

            return base.TrySendCommand(sender, new object[] { x, y, z, blockId }, out _);
        }
    }
}
