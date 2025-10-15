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
    public class ConditionalCommand : CommandBase<ConditionalResult>
    {
        public ConditionalCommand(TextTemplate executeIfCommand, LanguageManager languageManager)
        {
            ArgumentNullException.ThrowIfNull(executeIfCommand, nameof(executeIfCommand));

            Input = executeIfCommand;
            Output = languageManager["commands.execute.conditional.pass"];
            SuccessCountOutput = languageManager["commands.execute.conditional.pass_count"];
            FailedOutput = languageManager["commands.execute.conditional.fail"];
            FailedCountOutput = languageManager["commands.execute.conditional.fail_count"];
        }

        public override TextTemplate Input { get; }

        public override TextTemplate Output { get; }

        public TextTemplate SuccessCountOutput { get; }

        public TextTemplate FailedOutput { get; }

        public TextTemplate FailedCountOutput { get; }

        public virtual bool TrySendCommand(CommandSender sender, out ConditionalResult result)
        {
            return base.TrySendCommand(sender, Array.Empty<object>(), out result);
        }

        protected override bool TryMatchOutput(string output, [MaybeNullWhen(false)] out string[] outargs)
        {
            if (Output.TryMatch(output, out _))
            {
                outargs = [true.ToString(), 0.ToString()];
                return true;
            }
            else if (FailedOutput.TryMatch(output, out _))
            {
                outargs = [false.ToString(), 0.ToString()];
                return true;
            }
            else if (SuccessCountOutput.TryMatch(output, out var successCountArgs))
            {
                if (successCountArgs.Length == 1)
                {
                    outargs = [true.ToString(), successCountArgs[0]];
                    return true;
                }
            }
            else if (FailedCountOutput.TryMatch(output, out var failedCountArgs))
            {
                if (failedCountArgs.Length == 1)
                {
                    outargs = [false.ToString(), failedCountArgs[0]];
                    return true;
                }
            }

            outargs = null;
            return false;
        }

        protected override bool TryParseResult(string[] outargs, [MaybeNullWhen(false)] out ConditionalResult result)
        {
            if (outargs is not null &&
                outargs.Length == 2 &&
                bool.TryParse(outargs[0], out var isSuccess) &&
                int.TryParse(outargs[1], out var count))
            {
                result = new ConditionalResult(isSuccess, count);
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }
    }
}
