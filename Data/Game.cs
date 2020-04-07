using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class Game
    {
        public int Id { get; set; }
        public Config Player1 { get; set; }
        public Config Player2 { get; set; }
        public int BoardSize { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<Move> Moves { get; set; }
        public int Winner { get; set; }


        public Game()
        {
            Moves = new List<Move>();
        }
    }
}
