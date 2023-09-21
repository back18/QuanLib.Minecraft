using QuanLib.Minecraft.Command.Sender;
using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Model
{
    public class TitleTitleCommand : CountCommandBase
    {
        public TitleTitleCommand()
        {
            Input = TextTemplate.Parse("title %s title %s");
            Output = LanguageManager.Instance["commands.title.show.title.single"];
            CountOutput = LanguageManager.Instance["commands.title.show.title.multiple"];
        }

        public override TextTemplate Input { get; }

        public override TextTemplate Output { get; }

        public override TextTemplate CountOutput { get; }

        public bool TrySendCommand(CommandSender sender, string target, string message, out int result)
        {
            if (string.IsNullOrEmpty(target))
                throw new ArgumentException($"“{nameof(target)}”不能为 null 或空。", nameof(target));
            if (string.IsNullOrEmpty(message))
                throw new ArgumentException($"“{nameof(message)}”不能为 null 或空。", nameof(message));

            return base.TrySendCommand(sender, new object[] { target, message }, out result);
        }

        public override bool TryParseResult(string[] outargs, [MaybeNullWhen(false)] out int result)
        {
            return base.TryParseResult(outargs, 1, 0, out result);
        }
    }
}
