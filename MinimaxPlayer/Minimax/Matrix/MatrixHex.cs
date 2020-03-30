using System;
using MathNet.Numerics.LinearAlgebra;
using Players.Common;

namespace MinimaxPlayer.Minimax.Matrix
{
    public class MatrixHex
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public PlayerType Owner { get; set; }
        public Matrix<int> Neighbours { get; set; }
        public int Size { get; set; }

        public int Index
        {
            get => (Row * Size) + Column;
            set
            {
                Row = (int) value / Size;
                Column = value % Size;
            }
        }

        public MatrixHex(int boardSize)
        {
            Size = boardSize;
            Neighbours = Matrix<int>.Build.Dense(Size,Size);
            Neighbours[Row, Column] = 1;
        }
        
        public Tuple<int,int> ToTuple()
        {
            return new Tuple<int, int>(Row, Column);
        }

        public bool ReachesLeft()
        {
            for (var i = 0; i < Size; i++)
            {
                if (Neighbours[i, 0] > 0)
                {
                    return true;
                }
            }

            return false;
        }
        public bool ReachesTop()
        {
            for (var i = 0; i < Size; i++)
            {
                if (Neighbours[0, i] > 0)
                {
                    return true;
                }
            }

            return false;
        }
        public bool ReachesRight()
        {
            for (var i = 0; i < Size; i++)
            {
                if (Neighbours[i, Size - 1] > 0)
                {
                    return true;
                }
            }

            return false;
        }
        public bool ReachesBottom()
        {
            for (var i = 0; i < Size; i++)
            {
                if (Neighbours[Size - 1, i] > 0)
                {
                    return true;
                }
            }

            return false;
        }
        public void AttachTo(MatrixHex newNeighbour)
        {
            Neighbours[newNeighbour.Row, newNeighbour.Column] = 1;
        }
    }
}
