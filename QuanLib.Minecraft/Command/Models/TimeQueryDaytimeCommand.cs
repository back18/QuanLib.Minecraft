using QuanLib.Minecraft.ResourcePack.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command.Models
{
    public class TimeQueryDaytimeCommand : TimeQueryCommandBase
    {
        public TimeQueryDaytimeCommand()
        {
            Input = TextTemplate.Parse("time query daytime");
        }

        public override TextTemplate Input { get; }
    }
}
