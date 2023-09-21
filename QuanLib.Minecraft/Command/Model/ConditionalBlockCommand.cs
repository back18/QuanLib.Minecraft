using QuanLib.Minecraft.Command.Sender;
using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Model
{
    public class ConditionalBlockCommand : ConditionalCommandBase
    {
        public ConditionalBlockCommand()
        {
            Input = TextTemplate.Parse("execute if block %s %s %s %s");
        }

        public override TextTemplate Input { get; }

        public bool TrySendCommand(CommandSender sender, int x, int y, int z, string blockID)
        {
            if (string.IsNullOrEmpty(blockID))
                throw new ArgumentException($"“{nameof(blockID)}”不能为 null 或空。", nameof(blockID));

            return base.TrySendCommand(sender, new object[] { x, y, z, blockID }, out _);
        }
    }
}
