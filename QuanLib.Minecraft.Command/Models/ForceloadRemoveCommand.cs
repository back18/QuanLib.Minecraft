using QuanLib.Minecraft.Command.Senders;
using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Models
{
    public class ForceloadRemoveCommand : CommandBase, ICreatible<ForceloadRemoveCommand>
    {
        public ForceloadRemoveCommand(LanguageManager languageManager)
        {
            ArgumentNullException.ThrowIfNull(languageManager, nameof(languageManager));

            Output = languageManager["commands.forceload.removed.single"];
            Input = TextTemplate.Parse("forceload remove %s %s");
        }

        public override TextTemplate Input { get; }

        public override TextTemplate Output { get; }

        public bool TrySendCommand(CommandSender sender, int x, int z)
        {
            return base.TrySendCommand(sender, [x, z], out _);
        }

        public static ForceloadRemoveCommand Create(LanguageManager languageManager)
        {
            return new ForceloadRemoveCommand(languageManager);
        }
    }
}
