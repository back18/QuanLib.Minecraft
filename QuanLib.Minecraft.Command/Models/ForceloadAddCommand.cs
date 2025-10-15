using QuanLib.Minecraft.Command.Senders;
using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Models
{
    public class ForceloadAddCommand : CommandBase, ICreatible<ForceloadAddCommand>
    {
        public ForceloadAddCommand(LanguageManager languageManager)
        {
            ArgumentNullException.ThrowIfNull(languageManager, nameof(languageManager));

            Output = languageManager["commands.forceload.added.single"];
            Input = TextTemplate.Parse("forceload add %s %s");
        }

        public override TextTemplate Input { get; }

        public override TextTemplate Output { get; }

        public bool TrySendCommand(CommandSender sender, int x, int z)
        {
            return base.TrySendCommand(sender, [x, z], out _);
        }

        public static ForceloadAddCommand Create(LanguageManager languageManager)
        {
            return new ForceloadAddCommand(languageManager);
        }
    }
}
