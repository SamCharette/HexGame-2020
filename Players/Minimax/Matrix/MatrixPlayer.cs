using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Players.Base;
using Players.Common;
using Players.Minimax.List;

namespace Players.Minimax.Matrix
{
    public class MatrixPlayer : MinimaxPlayer
    {
        public new ListMap _memory;
        private int Size;
        private int _maxLevels;
        private readonly PlayerType _me;
        private int _nodesChecked;
        private PlayerType Opponent => _me == Common.PlayerType.Blue ? Common.PlayerType.Red : Common.PlayerType.Blue;

        public MatrixPlayer(int playerNumber, int boardSize, Config playerConfig) : base(playerNumber, boardSize, playerConfig)
        {
            Size = boardSize;
            PlayerNumber = playerNumber;
            _me = PlayerNumber == 1 ? Common.PlayerType.Blue : Common.PlayerType.Red;
            _maxLevels = GetDefault(playerConfig, "maxLevels", 20);
            Name = playerConfig.name;

            //Startup();
        }

        //        public override string PlayerType()
        //        {
        //            return "MiniMax Matrix AI (" + _maxLevels + ")";
        //        }

        //        public void Startup()
        //        {
        //            Quip("I will be looking " + _maxLevels + " deep.");
        //            _memory = new ListMap(Size);
        //        }

        //        public Matrix<int> MyMoves()
        //        {
        //            return PlayerNumber == 1 ? _memory.player1Matrix : _memory.player2Matrix;
        //        }

        //        public Matrix<int> EnemyMoves()
        //        {
        //            return PlayerNumber == 2 ? _memory.player1Matrix : _memory.player2Matrix;

        //        }

        //        public override Tuple<int, int> SelectHex(Tuple<int, int> opponentMove)
        //        {
        //            UpdateEnemyMoves(opponentMove);

        //            // Now that we've updated the player information, time to look for a move.
        //            var matrixToExamine = MyMoves()
        //                .Add(_memory.emptyMatrix.Multiply(2))
        //                .Add(EnemyMoves().Multiply(3));

        //            var currentScore = ScoreTheBoard(matrixToExamine, _me);

        //        }

        //        public int ScoreTheBoard(Matrix<int> board, PlayerType player)
        //        {
        //            // Note, the lower the score the better the score is
        //            var otherPlayer = player == Common.PlayerType.Blue ? Common.PlayerType.Red : Common.PlayerType.Blue;
        //            var playerScore = CostOfBestPath(board, player);
        //            var enemyScore = CostOfBestPath(board, otherPlayer);
        //            return playerScore - enemyScore;
        //        }

        //        public int CostOfBestPath(Matrix<int> board, PlayerType player)
        //        {
        //            // So the goal here is to find the best path from the board
        //            // for the player and return the number of hexes left to complete it.

        //            // Start at one edge, move to the next.  Empty spaces cost 1,
        //            // owned spaces cost 0 and are more desirable
        //            var nodesToLookAt = GetStartingHexes(board, player);

        //            // Something happened, let's just return a bad score.
        //            return 999;
        //        }

        //        public List<Tuple<int, int>> GetStartingHexes(Matrix<int> board, PlayerType player)
        //        {
        //            if (player == Common.PlayerType.Blue)
        //            {
        //                return TopFriendlyHexes(board);
        //            }
        //            else
        //            {
        //                return LeftFriendlyHexes(board);
        //            }
        //        }

        //        public List<Tuple<int, int>> TopFriendlyHexes(Matrix<int> board)
        //        {

        //        }

        //        public List<Tuple<int, int>> LeftFriendlyHexes(Matrix<int> board)
        //        {

        //        }
        //        private void UpdateEnemyMoves(Tuple<int, int> opponentMove)
        //        {
        //            if (opponentMove != null)
        //            {
        //                // First, let's set make note of the opponent's move
        //                if (PlayerNumber == 1)
        //                {
        //                    _memory.player2Matrix[opponentMove.Item1, opponentMove.Item2] = 1;
        //                }
        //                else
        //                {
        //                    _memory.player1Matrix[opponentMove.Item1, opponentMove.Item2] = 1;
        //                }
        //            }
        //        }

        //        public  Tuple<int, int> SelectHex(Tuple<int, int> opponentMove, int nothing)
        //        {
        //            _nodesChecked = 0;
        //            if (opponentMove != null)
        //            {
        //                // First we set the opponent's hex as being owned by them.
        //                var opponentHex = _memory.Board.FirstOrDefault(x => x.Row == opponentMove.Item1 && x.Column == opponentMove.Item2);
        //                if (opponentHex != null)
        //                {
        //                    _memory.TakeHex(Opponent, opponentMove.Item1, opponentMove.Item2);
        //                }
        //            }
        //            ListNode  choice = null;
        //            int bestScore = 999;

        //            var possibleMoves = _memory.Board.OrderBy(x => x.RandomValue).Where(x => x.Owner == Common.PlayerType.White);

        //            foreach (var move in possibleMoves)
        //            {
        //                var thoughtBoard = new ListMap(_memory);

        //                var scoreForThisMove = LetMeThinkAboutIt(thoughtBoard, _me, _maxLevels, 0, 0);
        //                if (scoreForThisMove < bestScore)
        //                {
        //                    bestScore = scoreForThisMove;
        //                    choice = _memory.Board.FirstOrDefault(x => x.Row == move.Row && x.Column == move.Column);
        //                }
        //            }

        //            Quip("Final moves checked out  : " + _nodesChecked);
        //            Quip("Best score found is " + bestScore);

        //            // And when in doubt, get a random one
        //            if (choice == null)
        //            {
        //                Quip("Random it is...");
        //                choice = _memory.Board.OrderBy(x => x.RandomValue)
        //                    .FirstOrDefault(x => x.Owner == Common.PlayerType.White);
        //            }

        //            _memory.TakeHex(_me, choice.Row, choice.Column);
        //            return new Tuple<int, int>(choice.Row, choice.Column);
        //        }

        //        private int ScoreFromBoard(PlayerType player, ListMap board)
        //        {
        //            // To score the board, we should find the best path for each player
        //            // and use them to determine the score.
        //            //
        //            // Any path with fewer hexes needed to get to an edge, for instance, is better
        //            if (board.Board.Count(x => x.Owner != Common.PlayerType.White) > 2)
        //            {

        //                var opponent = player == Common.PlayerType.Blue ? Common.PlayerType.Red : Common.PlayerType.Blue;

        //                var playerScore = board.Board.Where(x => x.Owner == player)
        //                    .OrderBy(y => y.RemainingDistance())
        //                    .FirstOrDefault();

        //                var opponentScore = board.Board.Where(x => x.Owner == opponent)
        //                    .OrderBy(y => y.RemainingDistance())
        //                    .FirstOrDefault();

        //                var finalScore = playerScore.RemainingDistance() - opponentScore.RemainingDistance();

        //                return finalScore;
        //            }

        //            return 0;

        //        }

        //        private int LetMeThinkAboutIt(ListMap thoughtBoard, PlayerType player, int depth, int alpha, int beta)
        //        {
        //            var currentAlpha = alpha;
        //            var currentBeta = beta;

        //            if (depth == 0 || thoughtBoard.Board.All(x => x.Owner != Common.PlayerType.White))
        //            {
        //                return ScoreFromBoard(player, thoughtBoard);
        //            }

        //            var possibleMoves = thoughtBoard.Board.Where(x => x.Owner == Common.PlayerType.White);
        //            // Get possible moves for player
        //            if (possibleMoves.Any())
        //            {
        //                var newThoughtBoard = new ListMap(thoughtBoard);

        //                if (player == _me)
        //                {
        //                    var bestValue = -999999;
        //                    foreach (var move in possibleMoves)
        //                    {

        //                        newThoughtBoard.TakeHex(player, move.Row, move.Column);
        //                        bestValue = Math.Max(bestValue, LetMeThinkAboutIt(newThoughtBoard, Opponent, depth - 1, currentAlpha, currentBeta));
        //                        currentAlpha = Math.Max(currentAlpha, bestValue);
        //                        if (currentBeta <= currentAlpha)
        //                        {
        //                            _nodesChecked++;
        //                            break;
        //                        }
        //                    }

        //                    return bestValue;
        //                } 
        //                else
        //                {
        //                    var bestValue = 999999;
        //                    foreach (var move in possibleMoves)
        //                    {

        //                        newThoughtBoard.TakeHex(player, move.Row, move.Column);
        //                        bestValue = Math.Min(bestValue, LetMeThinkAboutIt(newThoughtBoard, _me, depth - 1, currentAlpha, currentBeta));
        //                        currentBeta = Math.Min(currentBeta, bestValue);
        //                        if (currentBeta <= currentAlpha)
        //                        {
        //                            _nodesChecked++;
        //                            break;
        //                        }
        //                    }

        //                    return bestValue;
        //                }
        //            }
        //            else
        //            {
        //                return ScoreFromBoard(player, thoughtBoard);
        //            }
        //        }

    }
}
