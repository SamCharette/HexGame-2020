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
        private int _maxLevels = 121;
        private int _maxSeconds = 20;
        private readonly MinimaxGamePlayer _me;
        private MinimaxGamePlayer Opponent => _me == MinimaxGamePlayer.Blue ? MinimaxGamePlayer.Red : MinimaxGamePlayer.Blue;

        public MinimaxPlayer(int playerNumber, int boardSize) : base(playerNumber, boardSize)
        {
            PlayerNumber = playerNumber;
            _me = PlayerNumber == 1 ? MinimaxGamePlayer.Blue : MinimaxGamePlayer.Red;
            Size = boardSize;
            Startup();
        }

        public override string PlayerType()
        {
            return "MiniMax AI";
        }

        public void Startup()
        {
            _memory = new MinimaxMap(Size);
            _maxLevels = Size * Size;
        }

        public override Tuple<int, int> SelectHex(Tuple<int, int> opponentMove)
        {
            if (opponentMove != null)
            {
                // First we set the opponent's hex as being owned by them.
                var opponentHex = _memory.Board.FirstOrDefault(x => x.Row == opponentMove.Item1 && x.Column == opponentMove.Item2);
                if (opponentHex != null)
                {
                    _memory.TakeHex(Opponent, opponentMove.Item1, opponentMove.Item2);
                }
            }
            MinimaxNode  choice = null;
            int bestScore = -999;

            var possibleMoves = _memory.Board.OrderBy(x => x.RandomValue).Where(x => x.Owner == MinimaxGamePlayer.White);
            
            foreach (var move in possibleMoves)
            {
                var thoughtBoard = new MinimaxMap(_memory);

                var scoreForThisMove = LetMeThinkAboutIt(thoughtBoard, _me, _maxLevels, 0, 0);
                if (scoreForThisMove > bestScore)
                {
                    bestScore = scoreForThisMove;
                    choice = _memory.Board.FirstOrDefault(x => x.Row == move.Row && x.Column == move.Column);
                }
            }
            Quip("Best score found is " + bestScore);

            // And when in doubt, get a random one
            if (choice == null)
            {
                Quip("Random it is...");
                choice = _memory.Board.OrderBy(x => x.RandomValue)
                    .FirstOrDefault(x => x.Owner == MinimaxGamePlayer.White);
            }

            _memory.TakeHex(_me, choice.Row, choice.Column);
            return new Tuple<int, int>(choice.Row, choice.Column);
        }

        private int ScoreFromBoard(MinimaxGamePlayer player, MinimaxMap board)
        {
            // To score the board, we should find the best path for each player
            // and use them to determine the score.
            //
            // Any path with fewer hexes needed to get to an edge, for instance, is better
            if (_memory.Board.Count(x => x.Owner != MinimaxGamePlayer.White) > 2)
            {

                var opponent = player == MinimaxGamePlayer.Blue ? MinimaxGamePlayer.Red : MinimaxGamePlayer.Blue;

                var playerScore = board.Board.Where(x => x.Owner == player)
                    .OrderBy(y => y.RemainingDistance())
                    .FirstOrDefault();

                var opponentScore = board.Board.Where(x => x.Owner == opponent)
                    .OrderBy(y => y.RemainingDistance())
                    .FirstOrDefault();

                var finalScore = opponentScore.RemainingDistance() - playerScore.RemainingDistance();

                return finalScore;
            }

            return 0;

        }
        
        private int LetMeThinkAboutIt(MinimaxMap thoughtBoard, MinimaxGamePlayer player, int depth, int alpha, int beta)
        {
            var currentAlpha = alpha;
            var currentBeta = beta;

            if (depth == 0 || thoughtBoard.Board.All(x => x.Owner != MinimaxGamePlayer.White))
            {
                return ScoreFromBoard(player, thoughtBoard);
            }

            var possibleMoves = thoughtBoard.Board.Where(x => x.Owner == MinimaxGamePlayer.White);
            // Get possible moves for player
            if (possibleMoves.Any())
            {
                if (player == _me)
                {
                    var bestValue = -999999;
                    foreach (var move in possibleMoves)
                    {
                        
                        thoughtBoard.TakeHex(player, move.Row, move.Column);
                        bestValue = Math.Max(bestValue, LetMeThinkAboutIt(thoughtBoard, Opponent, depth - 1, currentAlpha, currentBeta));
                        currentAlpha = Math.Max(currentAlpha, bestValue);
                        if (currentBeta <= currentAlpha)
                        {
                            break;
                        }
                    }

                    return bestValue;
                } 
                else
                {
                    var bestValue = 999999;
                    foreach (var move in possibleMoves)
                    {
                        thoughtBoard.TakeHex(player, move.Row, move.Column);
                        bestValue = Math.Min(bestValue, LetMeThinkAboutIt(thoughtBoard, _me, depth - 1, currentAlpha, currentBeta));
                        currentBeta = Math.Min(currentBeta, bestValue);
                        if (currentBeta <= currentAlpha)
                        {
                            break;
                        }
                    }

                    return bestValue;
                }
            }
            else
            {
                return ScoreFromBoard(player, thoughtBoard);
            }
        }

    }
}
