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
    public class TelePortLocationCommand : CountCommandBase
    {
        public TelePortLocationCommand()
        {
            Input = TextTemplate.Parse("tp %s %s %s %s");
            Output = LanguageManager.Instance["commands.teleport.success.location.single"];
            CountOutput = LanguageManager.Instance["commands.teleport.success.location.multiple"];
        }

        public override TextTemplate Input { get; }

        public override TextTemplate Output { get; }

        public override TextTemplate CountOutput { get; }

        public bool TrySendCommand(CommandSender sender, string source, double x, double y, double z, out int result)
        {
            ArgumentException.ThrowIfNullOrEmpty(source, nameof(source));

            return base.TrySendCommand(sender, new object[] { source, x, y, z }, out result);
        }

        public override bool TryParseResult(string[] outargs, [MaybeNullWhen(false)] out int result)
        {
            return base.TryParseResult(outargs, 4, 0, out result);
        }
    }
}
