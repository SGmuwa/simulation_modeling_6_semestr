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
            while(!IsNeedStop)
            {
                stopwatch.Start();
                state.Update(stopwatch.Elapsed);
                if (false)
                {
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine(string.Join("\n", Table));
                    Console.WriteLine(state);
                    Console.Write(new string(' ', 128));
                }
                stopwatch.Restart();
            }
        }

        public void Stop()
        {
            IsNeedStop = true;
        }
    }
}
