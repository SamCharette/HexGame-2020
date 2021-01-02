using System;
using System.Collections.Generic;
using System.Text;
using Players;

namespace MonteCarloPlayer.Board
{
    public class Vertex
    {
        public int Row { get; set; }
        public int Column { get; set; } 

        public Vertex Parent = null;
        public int G { get; set; }
        public int H { get; set; }
        public PlayerType Owner { get; set; }
        public Status Status { get; set; }

        public Vertex()
        {

        }

        public Vertex(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }
}
