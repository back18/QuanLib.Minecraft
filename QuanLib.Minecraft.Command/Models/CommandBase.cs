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
    public abstract class CommandBase
    {
        public abstract TextTemplate Input { get; }

        public abstract TextTemplate Output { get; }

        public virtual bool TrySendCommand(CommandSender sender, object[] inargs, [MaybeNullWhen(false)] out string[] outargs)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            if (!Input.TryFormat(inargs, out var input))
                goto fail;

            string output = sender.SendCommand(input);
            if (!Output.TryMatch(output, out outargs))
                goto fail;

            return true;

            fail:
            outargs = null;
            return false;
        }
    }
}
