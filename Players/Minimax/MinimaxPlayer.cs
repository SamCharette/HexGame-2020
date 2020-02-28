using System;
using System.Collections.Generic;
using System.Linq;
using Players.Base;
using Players.Common;

namespace Players.Minimax
{
    public class MinimaxPlayer : Player
    {
        private new MinimaxMap _memory;
        private int Size;
        private int _maxLevels;
        private int _maxSeconds;
        private readonly MinimaxGamePlayer _me;
        private int _nodesChecked;
        private MinimaxGamePlayer Opponent => _me == MinimaxGamePlayer.Blue ? MinimaxGamePlayer.Red : MinimaxGamePlayer.Blue;

        public MinimaxPlayer(int playerNumber, int boardSize, Config playerConfig) : base(playerNumber, boardSize, playerConfig)
        {
            PlayerNumber = playerNumber;
            _me = PlayerNumber == 1 ? MinimaxGamePlayer.Blue : MinimaxGamePlayer.Red;
            Size = boardSize;
            _maxLevels = GetDefault(playerConfig, "maxLevels", 20);
            Name = playerConfig.name;

            Startup();
        }

        public override string PlayerType()
        {
            return "MiniMax AI (" + _maxLevels + ")";
        }

        public void Startup()
        {
            _memory = new MinimaxMap(Size);
        }

        public override Tuple<int, int> SelectHex(Tuple<int, int> opponentMove)
        {
            _nodesChecked = 0;
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
            int bestScore = 999;

            var possibleMoves = _memory.Board.OrderBy(x => x.RandomValue).Where(x => x.Owner == MinimaxGamePlayer.White);
            
            foreach (var move in possibleMoves)
            {
                var thoughtBoard = new MinimaxMap(_memory);

                var scoreForThisMove = LetMeThinkAboutIt(thoughtBoard, _me, _maxLevels, 0, 0);
                if (scoreForThisMove < bestScore)
                {
                    bestScore = scoreForThisMove;
                    choice = _memory.Board.FirstOrDefault(x => x.Row == move.Row && x.Column == move.Column);
                }
            }
 
            Quip("Final moves checked out  : " + _nodesChecked);
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
            if (board.Board.Count(x => x.Owner != MinimaxGamePlayer.White) > 2)
            {

                var opponent = player == MinimaxGamePlayer.Blue ? MinimaxGamePlayer.Red : MinimaxGamePlayer.Blue;

                var playerScore = board.Board.Where(x => x.Owner == player)
                    .OrderBy(y => y.RemainingDistance())
                    .FirstOrDefault();

                var opponentScore = board.Board.Where(x => x.Owner == opponent)
                    .OrderBy(y => y.RemainingDistance())
                    .FirstOrDefault();

                var finalScore = playerScore.RemainingDistance() - opponentScore.RemainingDistance();

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
                var newThoughtBoard = new MinimaxMap(thoughtBoard);
                if (player == _me)
                {
                    var bestValue = -999999;
                    foreach (var move in possibleMoves)
                    {
                        
                        newThoughtBoard.TakeHex(player, move.Row, move.Column);
                        bestValue = Math.Max(bestValue, LetMeThinkAboutIt(newThoughtBoard, Opponent, depth - 1, currentAlpha, currentBeta));
                        currentAlpha = Math.Max(currentAlpha, bestValue);
                        if (currentBeta <= currentAlpha)
                        {
                            _nodesChecked++;
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
                        
                        newThoughtBoard.TakeHex(player, move.Row, move.Column);
                        bestValue = Math.Min(bestValue, LetMeThinkAboutIt(newThoughtBoard, _me, depth - 1, currentAlpha, currentBeta));
                        currentBeta = Math.Min(currentBeta, bestValue);
                        if (currentBeta <= currentAlpha)
                        {
                            _nodesChecked++;
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
