using QuanLib.Minecraft.Command.Senders;
using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Models
{
    public class SetBlockCommand : CommandBase
    {
        public SetBlockCommand()
        {
            Input = TextTemplate.Parse("setblock %s %s %s %s");
            Output = LanguageManager.Instance["commands.setblock.success"];
        }

        public override TextTemplate Input { get; }

        public override TextTemplate Output { get; }

        public bool TrySendCommand(CommandSender sender, int x, int y, int z, string blockID)
        {
            ArgumentNullException.ThrowIfNull(blockID, nameof(blockID));

            return base.TrySendCommand(sender, new object[] { x, y, z, blockID }, out _);
        }
    }
}
