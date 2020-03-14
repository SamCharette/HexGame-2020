using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Players.Base;
using Players.Common;

namespace Players.Minimax.Matrix
{
    public class MatrixPlayer : MinimaxPlayer
    {
        public MatrixNode[,] Board { get; set; }
        public Matrix<int> MyMoves { get; set; }
        public Matrix<int> EnemyMoves { get; set; }
        public Matrix<int> EmptyMatrix { get; set; }
        public int MaxLevels { get; set; }
        private const int AbsoluteBestScore = 9999;
        private const int AbsoluteWorstScore = -9999;
        
        public int NodesChecked { get; set; }
        public PlayerType Opponent => Me == Common.PlayerType.Blue ? Common.PlayerType.Red : Common.PlayerType.Blue;

        public MatrixPlayer(int playerNumber, int boardSize, Config playerConfig) : base(playerNumber, boardSize, playerConfig)
        {
            Size = boardSize;
            PlayerNumber = playerNumber;
            Me = PlayerNumber == 1 ? Common.PlayerType.Blue : Common.PlayerType.Red;
            MaxLevels = GetDefault(playerConfig, "maxLevels", 20);
            Name = playerConfig.name;
            Board = new MatrixNode[Size,Size];
            for (int col = 0; col < Size; col++)
            {
                for (int row = 0; row < Size; row++)
                {
                    Board[row, col] = new MatrixNode(Size);
                } 
            }
            //Startup();
        }

        public override string PlayerType()
        {
            return "MiniMax Matrix AI (" + MaxLevels + ")";
        }

        public override Tuple<int, int> SelectHex(Tuple<int, int> opponentMove)
        {
            UpdateEnemyMoves(opponentMove);

            // Now that we've updated the player information, time to look for a move.
            var matrixToExamine = MyMoves
                .Add(EmptyMatrix.Multiply(2))
                .Add(EnemyMoves.Multiply(3));

            var bestScore = Minimax(matrixToExamine, MaxLevels, AbsoluteWorstScore, AbsoluteBestScore, true);


            return null;
        }

        public int Minimax(Matrix<int> board, int depth, int alpha, int beta, bool isMaximizing)
        {
            if (depth == 0 || IsThereAWinner(board, isMaximizing ? Me : Opponent))
            {
                return ScoreTheBoard(board, isMaximizing ? Me : Opponent);
            }

            if (isMaximizing)
            {
                var bestScore = AbsoluteWorstScore;
                // Get moves
                // for each possible move
                    // Take the hex for the player
                    // bestScore = Math.Max(bestScore(board, depth -1, alpha, beta, false);
                    // alpha = Math.Max(alpha, bestScore)
                    // Release the hex again
                    // if (beta <= alpha)
                    // {
                    //  break;
                    // }
                return bestScore;
            }
            else
            {
                var bestScore = AbsoluteBestScore;
                // Get Moves
                // for each possible move
                // Take the hex for the player
                // bestScore = Math.Min(bestScore(board, depth -1, alpha, beta, true);
                // alpha = Math.Min(alpha, bestScore)
                // Release the hex again
                // if (beta <= alpha)
                // {
                //  break;
                // }
                return bestScore;
            }
        }

        public int ScoreTheBoard(Matrix<int> board, PlayerType player)
        {
            if (IsThereAWinner(board, player))
            {
                return player == Me ? AbsoluteBestScore : AbsoluteWorstScore;
            }
            // Note, the lower the score the better the score is
            var otherPlayer = player == Common.PlayerType.Blue ? Common.PlayerType.Red : Common.PlayerType.Blue;
            var playerScore = CostOfBestPath(board, player);
            var enemyScore = CostOfBestPath(board, otherPlayer);
            return playerScore - enemyScore;
        }

        public int CostOfBestPath(Matrix<int> board, PlayerType player)
        {
            var vertical = MyMoves.RowSums();
            var horizontal = MyMoves.ColumnSums();
           
            var emptyVertical = vertical.Count(x => x.Equals(0));
            var emptyHorizontal = horizontal.Count(x => x.Equals(0));

            if (Me == Common.PlayerType.Blue)
            {
                return 1;
            }
            return (Size - emptyVertical) + (Size - emptyHorizontal);

        }

        public bool IsWinningPick(int newRow, int newColumn)
        {
            if (Me == Common.PlayerType.Blue)
            {
                return Board[newRow, newColumn].ReachesBottom() && Board[newRow, newColumn].ReachesTop();
            }
            return Board[newRow, newColumn].ReachesLeft() && Board[newRow, newColumn].ReachesRight();

        }

        public bool IsThereAWinner(Matrix<int> board, PlayerType player)
        {
            if (player == Common.PlayerType.Blue)
            {
                for (var i = 0; i < Size; i++)
                {
                    if (Board[0,i].Owner == player && Board[0, i].ReachesBottom())
                    {
                        return true;
                    }
                }
            } 
            else
            {
                for (var i = 0; i < Size; i++)
                {
                    if (Board[0, i].Owner == player && Board[i,0].ReachesRight())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void UpdateEnemyMoves(Tuple<int, int> opponentMove)
        {
            if (opponentMove != null)
            {
                EnemyMoves[opponentMove.Item1, opponentMove.Item2] = 1;
            }
        }

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
