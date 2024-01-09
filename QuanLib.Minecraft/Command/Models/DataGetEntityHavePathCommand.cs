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
    public class DataGetEntityHavePathCommand : DataGetEntityCommandBase
    {
        public DataGetEntityHavePathCommand()
        {
            Input = TextTemplate.Parse("data get entity %s %s");
        }

        public override TextTemplate Input { get; }

        public bool TrySendCommand(CommandSender sender, string target, string path, [MaybeNullWhen(false)] out string result)
        {
            return base.TrySendCommand(sender, new object[] { target, path }, out result);
        }
    }
}
