using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using Players.Common;

namespace Players.Minimax.Matrix
{

    public class SearchWorker
    {
        public int Size { get; set; }
        public int Depth { get; set; }
        public int MaxScore { get; set; }
        public int MinScore { get; set; }
        public bool IsMaximizing { get; set; }
        public Matrix<int> Memory { get; set; }
        public PlayerType Player { get; set; }

        public PlayerType Opponent => Player == PlayerType.Blue ? PlayerType.Red : PlayerType.Blue;

        public int PlayerNumber => Player == PlayerType.Blue ? 1 : 2;
        public int OpponentNumber => Player == PlayerType.Blue ? 2 : 1;
        public Tuple<int,int,int> MoveToExamine { get; set; }
        public int MoveScore { get; set; }

        public SearchWorker(int size, Tuple<int,int,int> move, int depth, int alpha, int beta, bool isMaximizing, Matrix<int> board)
        {
            Size = size;
            Depth = depth;
            MaxScore = alpha;
            MinScore = beta;
            IsMaximizing = isMaximizing;
            Memory = board;
            MoveToExamine = move;
        }

        public void Start()
        {
            
        }
        public bool IsThereAWinner(Matrix<int> board, PlayerType player)
        {
            var scout = new Scout();


            if (player == Common.PlayerType.Blue)
            {
                for (var startCol = 0; startCol < Size; startCol++)
                {
                    for (var endCol = 0; endCol < Size; endCol++)
                    {
                        var start = new Tuple<int,int>(0, startCol);
                        var end = new Tuple<int, int>(Size - 1, endCol);
                        if (scout.DoesPathExist(start, end, board))
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                for (var startRow = 0; startRow < Size; startRow++)
                {
                    for (var endRow = 0; endRow < Size; endRow++)
                    {
                        var start = new Tuple<int, int>(startRow, 0);
                        var end = new Tuple<int, int>(startRow, Size - 1);
                        if (scout.DoesPathExist(start, end, board))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        public int ScoreTheBoard(Matrix<int> board, PlayerType player)
        {
            if (IsThereAWinner(board, player))
            {
                return player == Player ? MaxScore : MinScore;
            }
            // Note, the lower the score the better the score is
            var otherPlayer = player == Common.PlayerType.Blue ? Common.PlayerType.Red : Common.PlayerType.Blue;
            var playerScore = CostOfBestPath(board, player);
            var enemyScore = CostOfBestPath(board, otherPlayer);
            return playerScore - enemyScore;
        }

        public int CostOfBestPath(Matrix<int> board, PlayerType player)
        {
            return 1;

        }

        public int Minimax(int depth, int alpha, int beta, bool isMaximizing)
        {
            if (depth == 0 || IsThereAWinner(Memory, isMaximizing ? Player : Opponent))
            {
                return ScoreTheBoard(Memory, isMaximizing ? Player : Opponent);
            }

            if (isMaximizing)
            {
                var bestScore = MinScore;
                var moves = Memory.EnumerateIndexed(Zeros.Include ).Where(x => x.Item3 == 0);
                foreach (var move in moves)
                {
                    Memory[move.Item1, move.Item2] = PlayerNumber;
                    bestScore = Math.Max(bestScore, Minimax(depth - 1, alpha, beta, false));
                    alpha = Math.Max(alpha, bestScore);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                return bestScore;
            }
            else
            {
                var bestScore = MaxScore;
                var moves = Memory.EnumerateIndexed(Zeros.Include).Where(x => x.Item3 == 0);
                foreach (var move in moves)
                {
                    Memory[move.Item1, move.Item2] = OpponentNumber;
                    bestScore = Math.Min(bestScore, Minimax(depth - 1, alpha, beta, false));
                    alpha = Math.Min(alpha, bestScore);
                    if (beta <= alpha)
                    {
                        break;
                    }
                }

                return bestScore;
            }
        }

    }
}
