using System;
using System.Collections.Generic;
using System.Text;

namespace Players.Minimax.List
{
    public class InquisitionParameters
    {
        public ListMap map;
        public List<ListHex> path;
        public ListHex currentMove;
        public int depth;
        public int alpha;
        public int beta;
        public bool isMaximizing;
    }
}
