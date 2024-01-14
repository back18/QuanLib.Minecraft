using QuanLib.Minecraft.Command.Senders;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Models
{
    public abstract class CommandBase<TResult> : CommandBase
    {
        public virtual bool TrySendCommand(CommandSender sender, object[] inargs, [MaybeNullWhen(false)] out TResult result)
        {
            if (!base.TrySendCommand(sender, inargs, out var outargs))
                goto fail;

            if (!TryParseResult(outargs, out result))
                goto fail;

            return true;

            fail:
            result = default;
            return false;
        }

        public abstract bool TryParseResult(string[] outargs, [MaybeNullWhen(false)] out TResult result);
    }
}
