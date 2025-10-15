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
    public abstract class CommandBase
    {
        public virtual string? Execute { get; set; }

        public abstract TextTemplate Input { get; }

        public abstract TextTemplate Output { get; }

        protected virtual bool TryFormatInput(object[] inargs, [MaybeNullWhen(false)] out string input)
        {
            if (!Input.TryFormat(inargs, out input))
                return false;

            if (!string.IsNullOrEmpty(Execute))
            {
                if (Execute.EndsWith(' '))
                    input = Execute + input;
                else
                    input = $"{Execute} {input}";
            }

            return true;
        }

        protected virtual bool TryMatchOutput(string output, [MaybeNullWhen(false)] out string[] outargs)
        {
            return Output.TryMatch(output, out outargs);
        }

        protected virtual bool TrySendCommand(CommandSender sender, object[] inargs, [MaybeNullWhen(false)] out string[] outargs)
        {
            ArgumentNullException.ThrowIfNull(sender, nameof(sender));

            if (!TryFormatInput(inargs, out var input))
                goto fail;

            string output = sender.SendCommand(input);

            if (!TryMatchOutput(output, out outargs))
                goto fail;

            return true;

            fail:
            outargs = null;
            return false;
        }
    }
}
