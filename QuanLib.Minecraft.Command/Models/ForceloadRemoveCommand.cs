using QuanLib.Minecraft.CommandSenders;
using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Models
{
    public class ForceloadRemoveCommand : CommandBase
    {
        public ForceloadRemoveCommand()
        {
            Input = TextTemplate.Parse("forceload remove %s %s");
            Output = LanguageManager.Instance["commands.forceload.removed.single"];
        }

        public override TextTemplate Input { get; }

        public override TextTemplate Output { get; }

        public bool TrySendCommand(CommandSender sender, int x, int z)
        {
            return base.TrySendCommand(sender, new object[] { x, z }, out _);
        }
    }
}
