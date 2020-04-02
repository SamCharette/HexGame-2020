using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NegamaxPlayer
{
    public class PriorityQueue
    {
        private ConcurrentDictionary<Hex, int> queue = new ConcurrentDictionary<Hex, int>();

        public void Push(Hex hex)
        {
            if (hex.Owner == 0)
            {
                if (queue.ContainsKey(hex))
                {
                    queue[hex]++;
                }
                else
                {
                    queue.TryAdd(hex, 0);
                }
            }
        }

        public bool HasItems()
        {
            return queue.Count > 0;
        }
        public int Clear()
        {
            var count = queue.Count;
            queue.Clear();
            return count;
        }

        public Hex Pop()
        {
            if (!queue.Any())
            {
                return null;
            }

            var hex = queue.OrderByDescending(x => x.Value).FirstOrDefault().Key;
            var value = 0;
            queue.TryRemove(hex, out value);
            return hex;

        }
    }
}
