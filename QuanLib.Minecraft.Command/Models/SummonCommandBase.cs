using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Models
{
    public abstract class SummonCommandBase : CommandBase
    {
        protected SummonCommandBase(LanguageManager languageManager)
        {
            ArgumentNullException.ThrowIfNull(languageManager, nameof(languageManager));

            Output = languageManager["commands.summon.success"];
        }

        public override TextTemplate Output { get; }
    }
}
