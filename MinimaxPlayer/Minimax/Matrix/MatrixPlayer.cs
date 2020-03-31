using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Players;

namespace MinimaxPlayer.Minimax.Matrix
{
    public class MatrixPlayer : MinimaxPlayer
    {
        public MatrixHex[,] Board { get; set; }
        public Matrix<int> MyMoves { get; set; }
        public Matrix<int> EnemyMoves { get; set; }
        public Matrix<int> EmptyMatrix { get; set; }
        public int MaxLevels { get; set; }
        private const int AbsoluteBestScore = 9999;
        private const int AbsoluteWorstScore = -9999;
        
        public PlayerType Opponent => Me == Players.PlayerType.Blue ? Players.PlayerType.Red : Players.PlayerType.Blue;

        public MatrixPlayer(int playerNumber, int boardSize, Config playerConfig) : base(playerNumber, boardSize, playerConfig)
        {
            Size = boardSize;
            PlayerNumber = playerNumber;
            Me = PlayerNumber == 1 ? Players.PlayerType.Blue : Players.PlayerType.Red;
            MaxLevels = GetDefault(playerConfig, "maxLevels", 20);
            Name = playerConfig.name;
            Board = new MatrixHex[Size,Size];
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    Board[row, col] = new MatrixHex(Size);
                } 
            }

            MyMoves = Matrix<int>.Build.Dense(Size, Size);
            MyMoves.Clear();
            EnemyMoves = Matrix<int>.Build.Dense(Size, Size);
            EnemyMoves.Clear();

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
            var otherPlayer = player == Players.PlayerType.Blue ? Players.PlayerType.Red : Players.PlayerType.Blue;
            var playerScore = CostOfBestPath(board, player);
            var enemyScore = CostOfBestPath(board, otherPlayer);
            return playerScore - enemyScore;
        }

        public int CostOfBestPath(Matrix<int> board, PlayerType player)
        {
            var vertical = MyMoves.RowSums();
            var horizontal = MyMoves.ColumnSums();
           
            var emptyVertical = Enumerable.Count<int>(vertical, x => x.Equals(0));
            var emptyHorizontal = Enumerable.Count<int>(horizontal, x => x.Equals(0));

            if (Me == Players.PlayerType.Blue)
            {
                return 1;
            }
            return (Size - emptyVertical) + (Size - emptyHorizontal);

        }

        public bool IsWinningPick(int newRow, int newColumn)
        {
            if (Me == Players.PlayerType.Blue)
            {
                return Board[newRow, newColumn].ReachesBottom() && Board[newRow, newColumn].ReachesTop();
            }
            return Board[newRow, newColumn].ReachesLeft() && Board[newRow, newColumn].ReachesRight();

        }

        public bool IsThereAWinner(Matrix<int> board, PlayerType player)
        {
            if (player == Players.PlayerType.Blue)
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
                EmptyMatrix[opponentMove.Item1, opponentMove.Item2] = 0;
            }
        }


    }
}
