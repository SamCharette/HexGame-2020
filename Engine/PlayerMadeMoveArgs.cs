using System;
using System.Collections.Generic;
using System.Text;
using Players.Common;

namespace Engine
{
    public class PlayerMadeMoveArgs : EventArgs
    {
        public int player;
        public Tuple<int, int> move;
    }
}
