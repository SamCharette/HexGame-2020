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

        public ListHex Parent;
        public Guid RandomValue;


        public Matrix<double> Attached { get; set; }
        public bool AttachedToTop { get; set; }
        public bool AttachedToLeft { get; set; }
        public bool AttachedToBottom { get; set; }
        public bool AttachedToRight { get; set; }



        public int G { get; set; }
        public int H { get; set; }
        public PlayerType Owner { get; set; }
        public Status Status { get; set; }
        
    
        public ListHex(int size, int row, int column) : base(size, row, column)
        {
            Parent = null;
            RandomValue = Guid.NewGuid();
            Status = Status.Untested;
            Attached = Matrix<double>.Build.Dense(size, size, 0);
            G = 0;
            H = 0;
            Console.WriteLine(Attached.ToString());
            Attached.At(row, column, 1.0);
            Owner = PlayerType.White;
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

        public bool IsAttachedToBothEnds(PlayerType player)
        {
            if (player == PlayerType.Blue)
            {
                return AttachedToTop && AttachedToBottom;
            }

            return AttachedToLeft && AttachedToRight;
        }

        private void SetEdgeAttachedStatuses()
        {
            SetColumnAttachedStatuses();
            SetRowAttachedStatuses();
        }

        private void SetColumnAttachedStatuses()
        {
            var columns = Attached.EnumerateColumnsIndexed(0, Size).ToList();
            AttachedToLeft = Attached.Column(0).Sum()  > 0;
            AttachedToRight = Attached.Column(Size - 1).Sum() > 0;
        }

        private void SetRowAttachedStatuses()
        {
            var rows = Attached.EnumerateRowsIndexed(0, Size).ToList();
            AttachedToTop = Attached.Row(0).Sum() > 0;
            AttachedToBottom = Attached.Row(Size - 1).Sum() > 0;
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
    }
}