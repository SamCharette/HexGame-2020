using System;
using System.Collections.Concurrent;

namespace Players
{
    public class PerformanceEventArgs : EventArgs
    {
        public int PlayerNumber;
        public ConcurrentDictionary<string,int> Counters = new ConcurrentDictionary<string, int>();
    }
}
