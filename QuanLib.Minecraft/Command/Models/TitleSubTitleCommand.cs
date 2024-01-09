using QuanLib.Minecraft.CommandSenders;
using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Models
{
    public class TitleSubTitleCommand : CountCommandBase
    {
        public TitleSubTitleCommand()
        {
            Input = TextTemplate.Parse("title %s subtitle %s");
            Output = LanguageManager.Instance["commands.title.show.subtitle.single"];
            CountOutput = LanguageManager.Instance["commands.title.show.subtitle.multiple"];
        }

        public override TextTemplate Input { get; }

        public override TextTemplate Output { get; }

        public override TextTemplate CountOutput { get; }

        public bool TrySendCommand(CommandSender sender, string target, string message, out int result)
        {
            ArgumentException.ThrowIfNullOrEmpty(target, nameof(target));
            ArgumentException.ThrowIfNullOrEmpty(message, nameof(message));

            return base.TrySendCommand(sender, new object[] { target, message }, out result);
        }

        public override bool TryParseResult(string[] outargs, [MaybeNullWhen(false)] out int result)
        {
            return base.TryParseResult(outargs, 1, 0, out result);
        }
    }
}
