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
    public class TelePortEntityCommand : CountCommandBase
    {
        public TelePortEntityCommand(LanguageManager languageManager)
        {
            ArgumentNullException.ThrowIfNull(languageManager, nameof(languageManager));

            CountOutput = languageManager["commands.teleport.success.entity.multiple"];
            Output = languageManager["commands.teleport.success.entity.single"];
            Input = TextTemplate.Parse("%s tp %s");
        }

        public override TextTemplate Input { get; }

        public override TextTemplate Output { get; }

        public override TextTemplate CountOutput { get; }

        public bool TrySendCommand(CommandSender sender, string source, string target, out int result)
        {
            ArgumentException.ThrowIfNullOrEmpty(source, nameof(source));
            ArgumentException.ThrowIfNullOrEmpty(target, nameof(target));

            return base.TrySendCommand(sender, new object[] { source, target }, out result);
        }

        public override bool TryParseResult(string[] outargs, [MaybeNullWhen(false)] out int result)
        {
            return base.TryParseResult(outargs, 2, 0, out result);
        }
    }
}
