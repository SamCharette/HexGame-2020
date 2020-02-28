using System;
using System.Collections.Generic;
using System.Linq;
using Players.Base;

namespace Players.Minimax
{
    public class MinimaxPlayer : Player
    {
        private new MinimaxMap _memory;
        private int Size;
        private int _maxLevels = 10;
        private int _maxSeconds = 20;
        private MinimaxGamePlayer Me;
        private MinimaxGamePlayer Opponent => Me == MinimaxGamePlayer.Blue ? MinimaxGamePlayer.Red : MinimaxGamePlayer.Blue;

        public MinimaxPlayer(int playerNumber, int boardSize) : base(playerNumber, boardSize)
        {
            PlayerNumber = playerNumber;
            Me = PlayerNumber == 1 ? MinimaxGamePlayer.Blue : MinimaxGamePlayer.Red;
            Size = boardSize;
            Startup();
        }

        public void Startup()
        {
            _memory = new MinimaxMap(Size);
            
        }

        public override Tuple<int, int> SelectHex(Tuple<int, int> opponentMove)
        {
            if (opponentMove != null)
            {
                // First we set the opponent's hex as being owned by them.
                var opponentHex = _memory.Board.FirstOrDefault(x => x.Row == opponentMove.Item1 && x.Column == opponentMove.Item2);
                if (opponentHex != null)
                {
                    opponentHex.Owner = Opponent;
                }
            }
            MinimaxNode  choice = null;
            // Next we need to take a look at the board state, evaluate it, and start looking
            // for good choices
            var score = ScoreFromBoard(Me);
            // And when in doubt, get a random one
            if (choice == null)
            {
                choice = _memory.Board.OrderBy(x => x.RandomValue)
                    .FirstOrDefault(x => x.Owner == MinimaxGamePlayer.White);
            }

            _memory.TakeHex(Me, choice.Row, choice.Column);
            return new Tuple<int, int>(choice.Row, choice.Column);
        }

        private int ScoreFromBoard(MinimaxGamePlayer player)
        {
            // To score the board, we should find the best path for each player
            // and use them to determine the score.
            //
            // Any path with fewer hexes needed to get to an edge, for instance, is better
            if (_memory.Board.Count(x => x.Owner != MinimaxGamePlayer.White) > 2)
            {
                var myScore = _memory.Board.Where(x => x.Owner == Opponent).OrderBy(y => y.RemainingDistance())
                    .FirstOrDefault();
                var notMyScore = _memory.Board.Where(x => x.Owner == Me).OrderBy(y => y.RemainingDistance())
                    .FirstOrDefault();

                var finalScore = myScore.RemainingDistance() - notMyScore.RemainingDistance();
                Quip("Score is " + finalScore);
                return finalScore;
            }

            return 0;

        }

    }
}
