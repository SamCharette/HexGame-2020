using System;
using System.Collections.Generic;
using System.Linq;
using Players.Base;
using Players.Common;
using Players.Minimax.List;

namespace Players.Minimax.List
{
    public class ListPlayer : MinimaxPlayer
    {
        private new ListMap _memory;
        private int Size;
        private int _maxLevels;
        private int _maxSeconds;
        private int _nodesChecked;
        private PlayerType Opponent => Me == Common.PlayerType.Blue  ? Common.PlayerType.Red : Common.PlayerType.Blue;

        public ListPlayer(int playerNumber, int boardSize, Config playerConfig) : base(playerNumber, boardSize, playerConfig)
        {
            
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
            _memory = new ListMap(Size);
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
            ListNode choice = null;
            int bestScore = 999;

            var possibleMoves = _memory.Board.OrderBy(x => x.RandomValue).Where(x => x.Owner == Common.PlayerType.White);

            foreach (var move in possibleMoves)
            {
                var thoughtBoard = new ListMap(_memory);

                var scoreForThisMove = LetMeThinkAboutIt(thoughtBoard, Me, _maxLevels, 0, 0);
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
                    .FirstOrDefault(x => x.Owner == Common.PlayerType.White);
            }

            _memory.TakeHex(Me, choice.Row, choice.Column);
            return new Tuple<int, int>(choice.Row, choice.Column);
        }

        private int ScoreFromBoard(Common.PlayerType player, ListMap board)
        {
            // To score the board, we should find the best path for each player
            // and use them to determine the score.
            //
            // Any path with fewer hexes needed to get to an edge, for instance, is better
            if (board.Board.Count(x => x.Owner != Common.PlayerType.White) > 2)
            {

                var opponent = player == Common.PlayerType.Blue ? Common.PlayerType.Red : Common.PlayerType.Blue;

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

        private int LetMeThinkAboutIt(ListMap thoughtBoard, Common.PlayerType player, int depth, int alpha, int beta)
        {
            var currentAlpha = alpha;
            var currentBeta = beta;

            if (depth == 0 || thoughtBoard.Board.All(x => x.Owner != Common.PlayerType.White))
            {
                return ScoreFromBoard(player, thoughtBoard);
            }

            var possibleMoves = thoughtBoard.Board.Where(x => x.Owner == Common.PlayerType.White);
            // Get possible moves for player
            if (possibleMoves.Any())
            {
                var newThoughtBoard = new ListMap(thoughtBoard);
                if (player == Me)
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
                        bestValue = Math.Min(bestValue, LetMeThinkAboutIt(newThoughtBoard, Me, depth - 1, currentAlpha, currentBeta));
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