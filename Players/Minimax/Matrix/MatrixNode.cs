using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using MathNet.Numerics.LinearAlgebra.Complex;

namespace Players.Minimax.Matrix
{
    public class MatrixNode
    {
        public int Row { get; set; }
        public int Column { get; set; }
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

        public MatrixNode(int boardSize)
        {
            Size = boardSize;
            Neighbours = Matrix<int>.Build.Dense(Size,Size);
        }


        public void AttachTo(MatrixNode newNeighbour)
        {
            Neighbours[newNeighbour.Row, newNeighbour.Column] = 1;
        }
    }
}
