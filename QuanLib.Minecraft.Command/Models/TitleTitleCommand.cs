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
    public class TitleTitleCommand : MultipleCommandBase, ICreatible<TitleTitleCommand>
    {
        public TitleTitleCommand(LanguageManager languageManager)
        {
            ArgumentNullException.ThrowIfNull(languageManager, nameof(languageManager));

            MultipleOutput = languageManager["commands.title.show.title.multiple"];
            Output = languageManager["commands.title.show.title.single"];
            Input = TextTemplate.Parse("title %s title %s");
        }

        public override TextTemplate Input { get; }

        public override TextTemplate Output { get; }

        public override TextTemplate MultipleOutput { get; }

        public bool TrySendCommand(CommandSender sender, string target, string message, out int result)
        {
            ArgumentException.ThrowIfNullOrEmpty(target, nameof(target));
            ArgumentException.ThrowIfNullOrEmpty(message, nameof(message));

            return base.TrySendCommand(sender, [target, message], out result);
        }

        public override bool TryParseResult(string[] outargs, [MaybeNullWhen(false)] out int result)
        {
            return base.TryParseResult(outargs, 1, 0, out result);
        }

        public static TitleTitleCommand Create(LanguageManager languageManager)
        {
            return new TitleTitleCommand(languageManager);
        }
    }
}
