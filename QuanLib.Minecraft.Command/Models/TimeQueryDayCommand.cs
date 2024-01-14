using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Models
{
    public class TimeQueryDayCommand : TimeQueryCommandBase
    {
        public TimeQueryDayCommand(LanguageManager languageManager) : base(languageManager)
        {
            Input = TextTemplate.Parse("time query day");
        }

        public override TextTemplate Input { get; }
    }
}
