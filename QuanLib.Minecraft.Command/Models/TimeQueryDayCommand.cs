using QuanLib.Minecraft.Command.Building;
using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Models
{
    public class TimeQueryDayCommand : TimeQueryCommandBase, ICreatible<TimeQueryDayCommand>
    {
        public TimeQueryDayCommand(LanguageManager languageManager) : base(languageManager)
        {
            Input = TextTemplate.Parse("time query day");
        }

        public override TextTemplate Input { get; }

        public static TimeQueryDayCommand Create(LanguageManager languageManager)
        {
            return new TimeQueryDayCommand(languageManager);
        }
    }
}
