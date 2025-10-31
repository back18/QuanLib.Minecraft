using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.Command
{
    public readonly struct CommandInfo(string input, string output, long startTimeStamp, long endTimeStamp)
    {
        private readonly static long s_systemStartTick = DateTime.Now.Ticks - Environment.TickCount64 * TimeSpan.TicksPerMillisecond;

        public readonly string Input = input;

        public readonly string Output = output;

        public readonly long StartTimeStamp = startTimeStamp;

        public readonly long EndTimeStamp = endTimeStamp;

        public DateTime StartTime => new(s_systemStartTick + StartTimeStamp);

        public DateTime EndTime => new(s_systemStartTick + EndTimeStamp);
    }
}
