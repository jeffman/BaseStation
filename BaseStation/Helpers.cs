using System;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

namespace BaseStation
{
    public static class Helpers
    {
        public static void WaitMicro(int microseconds)
        {
            long ticks = Stopwatch.Frequency * microseconds / 1000000L;
            if (ticks == 0)
                ticks = 1;

            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedTicks < ticks)
            {
                Thread.SpinWait(1);
            }

            stopwatch.Stop();
        }

        public static IEnumerable<T> EnumerateEnum<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}
