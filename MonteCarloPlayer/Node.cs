using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using Players;

namespace MonteCarloPlayer
{
    public class Node
    {
        public Matrix<int> Board { get; set; }
        public int NumberOfSimulations { get; set; }
        public int Score { get; set; }
        public int PlayerNumber { get;set; }
        public int Row { get; set; }
        public int Column { get; set; } 
        public Node Parent { get; set; }
        public List<Node> Children { get; set; }

        public Node(Matrix<int> gameState)
        {
            gameState.CopyTo(Board);
            Children = new List<Node>();
        }

        public Node GetBestChild()
        {
            if (Children.Count == 0)
            {
                return this;
            }

            return Children.OrderByDescending(x => x.UpperConfidenceValue()).FirstOrDefault();
        }

        public void UpdateBoard(Tuple<int, int, int> move, int playerNumber)
        {
            Board[move.Item1, move.Item2] = playerNumber;
            PlayerNumber = playerNumber;
        }
        public double UpperConfidenceValue()
        {
            if (NumberOfSimulations == 0)
            {
                return Int32.MaxValue;
            }
            return ((double)Score / (double)NumberOfSimulations)
                   + Math.Sqrt(Math.Log(Parent.NumberOfSimulations) / (double)NumberOfSimulations);
        }
    }
}
