using System;
using System.Collections.Generic;
using System.Linq;
using Players;

namespace MonteCarloPlayer
{
    public class MonteCarlo : Player
    {
        public Tuple<int,int> CurrentBestMove { get; set; }
        public List<Tuple<int,int,int>> Board { get; set; }
        private int RandomMovesMade = 0;
        private int MovesMade = 0;
        public MonteCarlo(int playerNumber, int boardSize, Config playerConfig) 
            : base(playerNumber, boardSize, playerConfig)
        {
            Size = boardSize;
            PlayerNumber = playerNumber;
            Setup(playerConfig);
        }


        public override Tuple<int, int> SelectHex(Tuple<int, int> opponentMove)
        {
            if (opponentMove != null)
            {
                // alter the in memory map so that the opponent's move is saved
            }

            CurrentBestMove = null;



            MovesMade++;
            return CheckBestMove();
        }

        private bool HexIsEmpty(Tuple<int, int> hex)
        {
            var newHex = Board?.FirstOrDefault(x =>
                x.Item1 == CurrentBestMove.Item1 && x.Item2 == CurrentBestMove.Item2);

            if (newHex != null)
            {
                return newHex.Item3 == 0;
            }

            return false;
        }

        private Tuple<int, int> CheckBestMove()
        {
            if (CurrentBestMove != null && HexIsEmpty(CurrentBestMove))
            {
              
                Quip("Um, for some reason I like the idea of taking an already taken spot " + CurrentBestMove + " (" + RandomMovesMade + " Random moves made)");
                CurrentBestMove = null;
            }
            if (CurrentBestMove == null)
            {
                //var openNodes = Board.Hexes.Where(x => x.Owner == 0);
                //var selectedNode = openNodes.OrderBy(x => Guid.NewGuid()).FirstOrDefault();

                //CurrentBestMove = selectedNode.ToTuple();
                RandomMovesMade++;
                Quip("Had to pick randomly.");
            }

            return CurrentBestMove;
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
