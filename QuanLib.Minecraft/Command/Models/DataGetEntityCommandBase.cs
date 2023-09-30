using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Models
{
    public abstract class DataGetEntityCommandBase : CommandBase<string>
    {
        public DataGetEntityCommandBase()
        {
            Output = LanguageManager.Instance["commands.data.entity.query"];
        }

        public override TextTemplate Output { get; }

        public override bool TryParseResult(string[] outargs, [MaybeNullWhen(false)] out string result)
        {
            if (outargs is not null && outargs.Length == 2)
            {
                result = outargs[1];
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
    }
}
