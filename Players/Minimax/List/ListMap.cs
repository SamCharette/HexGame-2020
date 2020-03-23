using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Players.Common;

namespace Players.Minimax.List
{
 
    public class ListMap
    {
        public int Size { get; set; }
        public List<ListHex> Board { get; set; }

        #region Constructors

        public ListMap()
        {
            Size = 11;
            CreateNewBoard();
        }

        public ListMap(int size)
        {
            Size = size;
            CreateNewBoard();
        }

        private void CreateNewBoard()
        {
            Board = new List<ListHex>(Size * Size);
            for (var row = 0; row < Size; row++)
            {
                for (var column = 0; column < Size; column++)
                {
                    var hex = new ListHex(Size, row, column);
                    Board.Add(hex);
                }
            }
        }

        #endregion

        #region HelperFunctions

        public Matrix<double> GetPlayerMatrix(PlayerType player)
        {
            var matrix = Matrix<double>.Build.Dense(Size, Size, 0);
            matrix.MapInplace(x => IsOwnedBy(x, player), Zeros.Include);
            return matrix;
        }

        private int IsOwnedBy(double index, PlayerType player)
        {
            var hex = HexAt((int)index);
            if (hex != null)
            {
                if (hex.Owner == player)
                {
                    return 1;
                }
            }
            return 0;
        }

        public ListHex HexAt(int index)
        {
            var row = (int)(index / Size);

            var column = index > 0 ?  (int)(index % Size) : index;
            return HexAt((int) row, (int) column);
        }
        public ListHex HexAt(int row, int column)
        {
            if (!IsInBounds(row, column))
            {
                return null;
            }

            return Board.FirstOrDefault(x => x.Row == row && x.Column == column);
        }

        public ListHex HexAt(Tuple<int, int> coordinates)
        {
            return HexAt(coordinates.Item1, coordinates.Item2);
        }

        public ListHex HexAt(ListHex hex)
        {
            return HexAt(hex.ToTuple());
        }

        private bool IsInBounds(int row, int column)
        {
            return row >= 0 && row < Size && column >= 0 && column < Size;
        }

        private bool IsInBounds(Tuple<int, int> coordinates)
        {
            return IsInBounds(coordinates.Item1, coordinates.Item2);
        }

        private bool IsInBounds(ListHex hex)
        {
            return IsInBounds(hex.ToTuple());
        }

        private List<ListHex> GetNeighboursFor(ListHex source)
        {
            var neighbours = new List<ListHex>();
            foreach (var neighbour in source.Neighbours)
            {
                var neighbourOnBoard = HexAt(neighbour.Row, neighbour.Column);
                if (neighbourOnBoard != null)
                {
                    neighbours.Add(neighbourOnBoard);
                }
            }

            return neighbours.ToList();
        }
        #endregion

        #region HexManipulation

        public bool TakeHex(PlayerType player, int row, int column)
        {
            if (!IsInBounds(row, column))
            {
                return false;
            }

            var hexToTake = Board.FirstOrDefault(x => x.Row == row && x.Column == column);
            if (hexToTake == null)
            {
                return false;
            }

            if (hexToTake.Owner != PlayerType.White)
            {
                return false;
            }

            hexToTake.Owner = player;
            var neighbours = GetNeighboursFor(hexToTake).Where(x => x.Owner == player).ToList();
            neighbours.Add(hexToTake);
            var newAttachedMatrix = Matrix<double>.Build.Dense(Size, Size, 0);
            foreach (var hex in neighbours)
            {
                newAttachedMatrix = newAttachedMatrix + hex.Attached;
            }

            // This is the new attached matrix
            newAttachedMatrix = newAttachedMatrix.PointwiseMinimum(1.0);

            // Every item marked 1 in the new attached matrix must be given the new
            // matrix, not just the neighbours
            var hexesToUpdate = newAttachedMatrix.EnumerateIndexed(Zeros.AllowSkip);
            foreach (var hex in hexesToUpdate)
            {
                var myHex = HexAt(hex.Item1, hex.Item2);
                myHex.Attached = newAttachedMatrix;
                myHex.SetEdgeAttachedStatuses();
            }

            return true;
        }

        #endregion


    }
}