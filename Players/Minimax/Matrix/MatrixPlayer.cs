using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MathNet.Numerics.LinearAlgebra;
using Players.Base;
using Players.Common;

namespace Players.Minimax.Matrix
{
    public class MatrixPlayer : MinimaxPlayer
    {
        public MatrixHex[,] Board { get; set; }
        public Matrix<int> MyMoves { get; set; }
        public Matrix<int> EnemyMoves { get; set; }
        public Matrix<int> EmptyMatrix { get; set; }
        private const int AbsoluteBestScore = 9999;
        private const int AbsoluteWorstScore = -9999;
        
        public PlayerType Opponent => Me == Common.PlayerType.Blue ? Common.PlayerType.Red : Common.PlayerType.Blue;

        public int OpponentNumber()
        {
            return Me == Common.PlayerType.Blue ? 2 : 1;
        }
        public MatrixPlayer(int playerNumber, int boardSize, Config playerConfig) : base(playerNumber, boardSize, playerConfig)
        {
            Size = boardSize;
            PlayerNumber = playerNumber;
            Me = PlayerNumber == 1 ? Common.PlayerType.Blue : Common.PlayerType.Red;
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
            var matrixToExamine = Matrix<int>.Build.Dense(Size, Size); 
            MyMoves
                .Add(EnemyMoves.Multiply(2))
                .CopyTo(matrixToExamine);
            var openSpots = matrixToExamine.EnumerateIndexed(Zeros.Include).Where(x => x.Item3 == 0);

            var bestScore = AbsoluteWorstScore;
            Tuple<int,int> moveToTake = null;
            
            foreach (var spot in openSpots)
            {
                var newMatrix = Matrix<int>.Build.Dense(Size, Size);
                var workerBee = new SearchWorker(Size, spot,MaxLevels, AbsoluteWorstScore, AbsoluteBestScore, true, newMatrix);
                workerBee.Start();
                if (workerBee.MoveScore > bestScore)
                {
                    moveToTake = new Tuple<int, int>(workerBee.MoveToExamine.Item1, workerBee.MoveToExamine.Item2);
                    bestScore = workerBee.MoveScore;
                } 
            }

            TakeHex(moveToTake);
            return moveToTake;
        }

        public void TakeHex(Tuple<int, int> move)
        {

        }
        private int GetRow(int index)
        {
            return (int)index / Size;
        }

        private int GetColomn(int index)
        {
            return index % Size;
        }

        public bool IsWinningPick(int newRow, int newColumn)
        {
            if (Me == Common.PlayerType.Blue)
            {
                return Board[newRow, newColumn].ReachesBottom() && Board[newRow, newColumn].ReachesTop();
            }
            return Board[newRow, newColumn].ReachesLeft() && Board[newRow, newColumn].ReachesRight();

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
