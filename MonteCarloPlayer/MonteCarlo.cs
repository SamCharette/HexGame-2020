using System;
using Players;

namespace MonteCarloPlayer
{
    public class MonteCarlo : Player
    {

        public MonteCarlo(int playerNumber, int boardSize, Config playerConfig) 
            : base(playerNumber, boardSize, playerConfig)
        {
            Size = boardSize;
            PlayerNumber = playerNumber;
            Setup(playerConfig);
        }


        public override Tuple<int, int> SelectHex(Tuple<int, int> opponentMove)
        {


            return null;
        }

        private void Setup(Config playerConfig)
        {

        }

        public override string CodeName()
        {
            return "Melece";
        }
    }
}
