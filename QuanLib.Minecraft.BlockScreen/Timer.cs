using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLib.Minecraft.BlockScreen
{
    public class Timer
    {
        public Timer()
        {
            _times_20 = new();
            _times_1200 = new();
            _times_6000 = new();
            _total_20 = _total_1200 = _total_6000 = TimeSpan.Zero;
        }

        private readonly Queue<TimeSpan> _times_20 = new();
        private readonly Queue<TimeSpan> _times_1200 = new();
        private readonly Queue<TimeSpan> _times_6000 = new();
        private TimeSpan _total_20;
        private TimeSpan _total_1200;
        private TimeSpan _total_6000;

        public TimeSpan LastTime { get; private set; }

        public TimeSpan AverageTime1Second
        {
            get
            {
                if (_times_20.Count == 0)
                    return TimeSpan.Zero;
                return _total_20 / _times_20.Count;
            }
        }

        public TimeSpan AverageTime1Minute
        {
            get
            {
                if (_times_1200.Count == 0)
                    return TimeSpan.Zero;
                return _total_1200 / _times_1200.Count;
            }
        }

        public TimeSpan AverageTime5Minute {
            get
            {
                if (_times_6000.Count == 0)
                    return TimeSpan.Zero;
                return _total_6000 / _times_6000.Count;
            }
        }

        public void Add(TimeSpan time)
        {
            if (_times_20.Count > 20)
                _total_20 -= _times_20.Dequeue();
            if (_times_1200.Count > 1200)
                _total_1200 -= _times_1200.Dequeue();
            if (_times_6000.Count > 6000)
                _total_6000 -= _times_6000.Dequeue();

            LastTime = time;
            _total_20 += time;
            _total_1200 += time;
            _total_6000 += time;

            _times_20.Enqueue(time);
            _times_1200.Enqueue(time);
            _times_6000.Enqueue(time);
        }

        public void Restart()
        {
            _times_20.Clear();
            _times_1200.Clear();
            _times_6000.Clear();
            _total_20 = _total_1200 = _total_6000 = TimeSpan.Zero;
        }

        public TimeSpan GetTime(Duration duration)
        {
            return duration switch
            {
                Duration.Last => LastTime,
                Duration.Tick20 => AverageTime1Second,
                Duration.Tick1200 => AverageTime1Minute,
                Duration.Tick6000 => AverageTime5Minute,
                _ => throw new InvalidOperationException(),
            };
        }

        public static string FromTime(TimeSpan time)
        {
            return Math.Round(time.TotalMilliseconds, 3).ToString();
        }

        public enum Duration
        {
            Last,

            Tick20,

            Tick1200,

            Tick6000
        }
    }
}
