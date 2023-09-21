using QuanLib.Minecraft.Command.Sender;
using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Model
{
    public class TellrawCommand : CommandBase
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
            if (string.IsNullOrEmpty(target))
                throw new ArgumentException($"“{nameof(target)}”不能为 null 或空。", nameof(target));
            if (string.IsNullOrEmpty(message))
                throw new ArgumentException($"“{nameof(message)}”不能为 null 或空。", nameof(message));

            return base.TrySendCommand(sender, new object[] {target, message}, out _);
        }
    }
}
