using QuanLib.Minecraft.Command.Sender;
using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Model
{
    public class ForceloadAddCommand : CommandBase
    {
        public ForceloadAddCommand()
        {
            Input = TextTemplate.Parse("forceload add %s %s");
            Output = LanguageManager.Instance["commands.forceload.added.single"];
        }

        public override TextTemplate Input { get; }

        public override TextTemplate Output { get; }

        public bool TrySendCommand(CommandSender sender, int x, int z)
        {
            return base.TrySendCommand(sender, new object[] { x, z }, out _);
        }
    }
}
