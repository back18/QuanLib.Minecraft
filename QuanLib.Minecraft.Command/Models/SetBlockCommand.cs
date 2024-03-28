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
        public SetBlockCommand(LanguageManager languageManager)
        {
            ArgumentNullException.ThrowIfNull(languageManager, nameof(languageManager));

            Output = languageManager["commands.setblock.success"];
            Input = TextTemplate.Parse("setblock %s %s %s %s");
        }

        public override TextTemplate Input { get; }

        public override TextTemplate Output { get; }

        public bool TrySendCommand(CommandSender sender, int x, int y, int z, string blockId)
        {
            ArgumentNullException.ThrowIfNull(blockId, nameof(blockId));

            return base.TrySendCommand(sender, new object[] { x, y, z, blockId }, out _);
        }
    }
}
