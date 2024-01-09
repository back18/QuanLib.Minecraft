using QuanLib.Minecraft.CommandSenders;
using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Models
{
    public class DataGetEntityCommand : DataGetEntityCommandBase
    {
        public DataGetEntityCommand()
        {
            Input = TextTemplate.Parse("data get entity %s");
        }

        public bool TrySendCommand(CommandSender sender, string target, [MaybeNullWhen(false)] out string result)
        {
            return base.TrySendCommand(sender, new object[] { target }, out result);
        }

        public override TextTemplate Input { get; }
    }
}
