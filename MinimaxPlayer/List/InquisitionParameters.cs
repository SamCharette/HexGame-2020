using System.Collections.Generic;

namespace MinimaxPlayer.List
{
    public class InquisitionParameters
    {
        public ListPlayer player;
        public ListMap map;
        public List<ListHex> path;
        public ListHex currentMove;
        public int depth;
        public int alpha;
        public int beta;
        public bool isMaximizing;
    }
}
