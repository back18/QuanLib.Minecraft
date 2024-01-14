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
    public class TitleTimesCommand : MultipleCommandBase
    {
        public TitleTimesCommand(LanguageManager languageManager)
        {
            ArgumentNullException.ThrowIfNull(languageManager, nameof(languageManager));

            MultipleOutput = languageManager["commands.title.times.multiple"];
            Output = languageManager["commands.title.times.single"];
            Input = TextTemplate.Parse("title %s times %s %s %s");
        }

        public override TextTemplate Input { get; }

        public override TextTemplate Output { get; }

        public override TextTemplate MultipleOutput { get; }

        public bool TrySendCommand(CommandSender sender, string target, int fadeIn, int stay, int fadeOut, out int result)
        {
            ArgumentException.ThrowIfNullOrEmpty(target, nameof(target));

            return base.TrySendCommand(sender, new object[] { target, fadeIn, stay, fadeOut }, out result);
        }

        public override bool TryParseResult(string[] outargs, [MaybeNullWhen(false)] out int result)
        {
            return base.TryParseResult(outargs, 1, 0, out result);
        }
    }
}
