using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace SystemDynamics
{
    class Drawer
    {
        public readonly WaterIceSteamState state
            = new WaterIceSteamState();
        public readonly List<string> Table
            = new List<string>();
        private bool IsNeedStop = true;
        private int intervalUpdate = 400;
        /// <summary>
        /// Промежуток обновления системы.
        /// Разрешено от 0 до 1000.
        /// </summary>
        public int IntervalUpdate
        {
            get => intervalUpdate;
            set
            {
                if (0 <= value && value <= 1000)
                    intervalUpdate = value;
            }
        }

        public void Run()
        {
            if (IsNeedStop == false)
                return;
            IsNeedStop = false;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while(!IsNeedStop)
            {
                TimeSpan span = stopwatch.Elapsed;
                stopwatch.Restart();
                state.Update(span);
                Console.Title = state.ToString(false);
                Thread.Sleep(IntervalUpdate);
            }
            stopwatch.Stop();
        }

        public void Stop()
        {
            IsNeedStop = true;
        }
    }
}
