using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public static class Utility
    {
        public static void BusyWaiting(TimeSpan time)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (stopwatch.Elapsed < time)
                Thread.Yield();
        }

        public static void BusyWaiting(int milliseconds)
        {
            if (milliseconds < 0)
                return;
            BusyWaiting(TimeSpan.FromMilliseconds(milliseconds));
        }
    }
}
