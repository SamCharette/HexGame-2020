using System.Collections.Generic;

namespace MinimaxPlayer.Minimax.List
{
    public class InquisitionParameters
    {
        public MinimaxPlayer player;
        public ListMap map;
        public List<ListHex> path;
        public ListHex currentMove;
        public int depth;
        public int alpha;
        public int beta;
        public bool isMaximizing;
    }
}
