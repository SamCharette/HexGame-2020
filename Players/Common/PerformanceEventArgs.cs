using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Players.Common
{
    public class PerformanceEventArgs : EventArgs
    {
        public int PlayerNumber;
        public ConcurrentDictionary<string,int> Counters = new ConcurrentDictionary<string, int>();
    }
}
