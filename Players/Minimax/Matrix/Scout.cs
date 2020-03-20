using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace Players.Minimax.Matrix
{
    public class Scout
    {
        private Matrix<int> Memory;
        public bool DoesPathExist(Tuple<int, int> start, Tuple<int, int> end, Matrix<int> board)
        {
            Memory = board;

            return false;
        }
    }
}
