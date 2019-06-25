using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SystemDynamics
{
    class Drawer
    {
        public readonly WaterIceSteamState state
            = new WaterIceSteamState();
        public readonly List<string> Table
            = new List<string>();
        private bool IsNeedStop = true;

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
            }
            stopwatch.Stop();
        }

        public void Stop()
        {
            IsNeedStop = true;
        }
    }
}
