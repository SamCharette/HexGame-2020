using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Players.Common;

namespace Players.Minimax.List
{
    public class ListHex : SimpleHex
    {
        public List<SimpleHex> Neighbours = new List<SimpleHex>();

        public ListHex Parent = null;
        public Guid RandomValue;


        public Matrix<double> Attached { get; set; }
        public bool IsAttachedToTop { get; set; }
        public bool IsAttachedToLeft { get; set; }
        public bool IsAttachedToBottom { get; set; }
        public bool IsAttachedToRight { get; set; }



        public int G { get; set; }
        public int H { get; set; }
        public PlayerType Owner { get; set; }
        public Status Status { get; set; }


        public ListHex()
        {
   

        }
        public ListHex(int size, int row, int column) : base(size, row, column)
        {
            Parent = null;
            RandomValue = Guid.NewGuid();
            Status = Status.Untested;
            Attached = Matrix<double>.Build.Dense(size, size, 0);
            G = 0;
            H = 0;
            
            Attached.At(row, column, 1.0);
            Owner = PlayerType.White;
            GetNeighbours();
            SetEdgeAttachedStatuses();
        }

        public void PostCloneWork()
        {
            GetNeighbours();
            SetEdgeAttachedStatuses();
        }

        private void GetNeighbours()
        {
            for (var i = 0; i < 6; i++)
            {
                var coordinates = AddDelta(Compass.GetCoordinatesFor(ToTuple(), i));

                var newNeighbour = new SimpleHex(Size, coordinates.Item1, coordinates.Item2);

                if (newNeighbour.IsInBounds())
                {
                    Neighbours.Add(newNeighbour);
                }
            }
        }

        public bool IsAttachedToBothEnds()
        {
            if (Owner == PlayerType.Blue)
            {
                return IsAttachedToTop && IsAttachedToBottom;
            } 
            else if (Owner == PlayerType.Red)
            {
                return IsAttachedToLeft && IsAttachedToRight;
            }

            return false;

        }

        public void SetEdgeAttachedStatuses()
        {
            SetColumnAttachedStatuses();
            SetRowAttachedStatuses();
        }

        private void SetColumnAttachedStatuses()
        {
            var columns = Attached.EnumerateColumnsIndexed(0, Size).OrderBy(x => x.Item1).ToList();
            IsAttachedToLeft = columns.First().Item2.Sum() > 0;
            IsAttachedToRight = columns.Last().Item2.Sum() > 0;
        }

        private void SetRowAttachedStatuses()
        {
            var rows = Attached.EnumerateRowsIndexed(0, Size).OrderBy(x => x.Item1).ToList();
            IsAttachedToTop = rows.First().Item2.Sum() > 0;
            IsAttachedToBottom = rows.Last().Item2.Sum() > 0;

        }

        public Tuple<int, int> AddDelta(Tuple<int, int> delta)
        {
            return new Tuple<int, int>(Row + delta.Item1, Column + delta.Item2);
        }

        public void AttachTo(ListHex node)
        {
            if (node != null && node.Owner == Owner)
            {
                Attached = Attached.Add(node.Attached).PointwiseMinimum(1.0);
            }
            SetEdgeAttachedStatuses();
        }

        public void ClearPathingVariables()
        {
            G = 0;
            H = 0;
            Parent = null;
            Status = Status.Untested;
        }

        public void DetachFrom(ListHex node)
        {
            if (node != null)
            {
                Attached[node.Row, node.Column] = 0;
                SetEdgeAttachedStatuses();
            }
        }

        public int F()
        {
            return G + H;
        }

        public bool IsAttachedTo(ListHex node)
        {
            if (node != null)
            {
                return IsAttachedTo(node.Row, node.Column);
            }

            return false;
        }

        public bool IsAttachedTo(int row, int column)
        {
            return Attached.At(row, column) == 1.0;
        }

        public string ToLongString()
        {
            var output = "";
            output += (ToString() + " - Owned by " + Owner);
            output += Environment.NewLine;
            output += "F => " + F();
            output += Environment.NewLine;
            output += "G => " + G;
            output += Environment.NewLine;
            output += "H => " + H;
            output += Environment.NewLine;
            output += "Random Value => " + RandomValue;
            return output;
        }
    }
}