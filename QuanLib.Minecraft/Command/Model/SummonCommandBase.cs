using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Model
{
    public abstract class SummonCommandBase : CommandBase
    {
        protected SummonCommandBase()
        {
            Output = LanguageManager.Instance["commands.summon.success"];
        }

        public override TextTemplate Output { get; }
    }
}
