using System;
using System.Collections.Generic;
using System.Text;
using Players;

namespace NegamaxPlayer
{
    public class Negamax : Player
    {
        public Board Board { get; set; }


        public Negamax(int playerNumber, int boardSize, Config playerConfig) : base(playerNumber, boardSize, playerConfig)
        {
            
        }

        public override Tuple<int, int> SelectHex(Tuple<int, int> opponentMove)
        {
            
        }

        public override string PlayerType()
        {
            return "Negamax AI";
        }
    }
}
