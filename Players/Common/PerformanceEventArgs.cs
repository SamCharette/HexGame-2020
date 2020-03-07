using System;
using System.Collections.Generic;
using System.Text;

namespace Players.Common
{
    public class PerformanceEventArgs : EventArgs
    {
        public int PlayerNumber;
        public Dictionary<string,int> Counters = new Dictionary<string, int>();
    }
}
