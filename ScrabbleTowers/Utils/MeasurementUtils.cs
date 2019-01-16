using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ScrabbleTowers.Utils
{
    public static class MeasurementUtils
    {
        public static async Task<TimeSpan> RunWithStopwatch(Task task)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await task;
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }
    }
}
