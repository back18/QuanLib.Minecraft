using QuanLib.Minecraft.Command.Senders;
using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Models
{
    public class TellrawCommand : CommandBase, ICreatible<TellrawCommand>
    {
        public TellrawCommand()
        {
            Input = TextTemplate.Parse("tellraw %s %s");
            Output = TextTemplate.Parse("%s");
        }

        public override TextTemplate Input { get; }

        public override TextTemplate Output { get; }

        public bool TrySendCommand(CommandSender sender, string target, string message)
        {
            ArgumentException.ThrowIfNullOrEmpty(target, nameof(target));
            ArgumentException.ThrowIfNullOrEmpty(message, nameof(message));

            return base.TrySendCommand(sender, [target, message], out _);
        }

        public static TellrawCommand Create(LanguageManager languageManager)
        {
            throw new NotImplementedException();
        }
    }
}
