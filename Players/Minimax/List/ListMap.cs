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
        public ListHex At(int row, int column)
        {
            if (!IsInBounds(row, column))
            {
                return null;
            }

            return Board.FirstOrDefault(x => x.Row == row && x.Column == column);
        }

        public ListHex At(Tuple<int, int> coordinates)
        {
            return At(coordinates.Item1, coordinates.Item2);
        }

        public ListHex At(ListHex hex)
        {
            return At(hex.ToTuple());
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
                var neighbourOnBoard = At(neighbour.Row, neighbour.Column);
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
            var neighbours = GetNeighboursFor(hexToTake);
            neighbours.Add(hexToTake);
            var newAttachedMatrix = Matrix<double>.Build.Dense(Size, Size, 0);
            foreach (var hex in neighbours)
            {
                newAttachedMatrix = newAttachedMatrix + hex.Attached;
            }

            newAttachedMatrix = newAttachedMatrix.PointwiseMaximum(1);
            foreach (var hex in neighbours)
            {
                hex.Attached = newAttachedMatrix;
            }


            return true;
        }

        #endregion


    }
}