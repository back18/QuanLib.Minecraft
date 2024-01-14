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
    public abstract class MultipleCommandBase : CommandBase<int>
    {
        public abstract TextTemplate MultipleOutput { get; }

        public override bool TrySendCommand(CommandSender sender, object[] inargs, [MaybeNullWhen(false)] out string[] outargs)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            if (!Input.TryFormat(inargs, out var input))
                goto fail;

            string output = sender.SendCommand(input);
            if (!Output.TryMatch(output, out outargs) && !MultipleOutput.TryMatch(output, out outargs))
                goto fail;

            fail:
            outargs = null;
            return false;
        }

        protected virtual bool TryParseResult(string[] outargs, int length, int index, [MaybeNullWhen(false)] out int result)
        {
            if (outargs is null || outargs.Length != length)
                goto fail;

            if (int.TryParse(outargs[index], out result))
            {
                return true;
            }
            else
            {
                result = 1;
                return true;
            }

            fail:
            result = default;
            return false;
        }
    }
}
