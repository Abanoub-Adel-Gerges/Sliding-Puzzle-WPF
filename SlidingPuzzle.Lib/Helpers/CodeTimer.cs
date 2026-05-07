using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SlidingPuzzle.Lib.Helpers
{
    public class CodeTimer
    {
        public static async Task<TimeSpan> MeasureAsync(Func<Task> action)
        {
            var sw = Stopwatch.StartNew();
            await action();
            sw.Stop();
            return sw.Elapsed;
        }
        public static TimeSpan Measure(Action action)
        {
            var sw = Stopwatch.StartNew();
            action();
            sw.Stop();
            return sw.Elapsed;
        }
    }
}
