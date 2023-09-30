using QuanLib.Minecraft.Command.Senders;
using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Models
{
    public abstract class TimeQueryCommandBase : CommandBase<int>
    {
        protected TimeQueryCommandBase()
        {
            Output = LanguageManager.Instance["commands.time.query"];
        }

        public override TextTemplate Output { get; }

        public virtual bool TrySendCommand(CommandSender sender, out int result)
        {
            return base.TrySendCommand(sender, Array.Empty<object>(), out result);
        }

        public override bool TryParseResult(string[] outargs, [MaybeNullWhen(false)] out int result)
        {
            if (outargs is not null && outargs.Length == 1 && int.TryParse(outargs[0], out result))
            {
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }
    }
}
