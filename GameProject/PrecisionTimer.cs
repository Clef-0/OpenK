using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace GameProject
{
    /// <summary>
    /// Replacement for DateTime.Now that returns a much more accurate time measurement by adding on a stopwatch value measured from the last DateTime update.
    /// </summary>
    public class PrecisionTimer
    {
        private static Stopwatch stopwatch = new Stopwatch();
        private static DateTime startTime;

        static PrecisionTimer()
        {
            Reset();
            SystemEvents.TimeChanged += ResetTimer;
        }

        static void ResetTimer(object sender, EventArgs e)
        {
            Reset();
        }

        static public void Reset()
        {
            startTime = DateTime.Now;
            stopwatch.Restart();
        }

        public DateTime Now {
            get {
                return startTime.Add(stopwatch.Elapsed);
            }
        }
    }
}
