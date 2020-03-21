using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Players.Common;

namespace Players.Minimax.List
{
    public class ListHex : SimpleHex
    {
        public List<SimpleHex> Neighbours;

        public ListHex Parent;
        public Guid RandomValue;


        public Matrix<int> Attached { get; set; }
        public bool AttachedToTop { get; set; }
        public bool AttachedToLeft { get; set; }
        public bool AttachedToBottom { get; set; }
        public bool AttachedToRight { get; set; }



        public int G { get; set; }
        public int H { get; set; }
        public PlayerType Owner { get; set; }
        public Status Status { get; set; }
        
        public ListHex(int size, int row, int column) : base(row, column)
        {
            Size = size;
            Parent = null;
            RandomValue = Guid.NewGuid();
            Status = Status.Untested;
            Attached = Matrix<int>.Build.Dense(size, size, 0);
            G = 0;
            H = 0;
            Row = row;
            Column = column;
            Attached[Row, Column] = 1;
            Owner = PlayerType.White;
            GetNeighbours();
            SetEdgeAttachedStatuses();
        }

        private void GetNeighbours()
        {
            for (var i = 0; i < 6; i++)
            {
                var newNeighbour = new SimpleHex(Compass.GetCoordinatesFor(ToTuple(), i));
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
            AttachedToLeft = columns.FirstOrDefault(x => x.Item1 == 0).Item2.Sum() > 0;
            AttachedToRight = columns.FirstOrDefault(x => x.Item1 == Size - 1).Item2.Sum() > 0;
        }

        private void SetRowAttachedStatuses()
        {
            var rows = Attached.EnumerateRowsIndexed(0, Size).ToList();
            AttachedToTop = rows.FirstOrDefault(x => x.Item1 == 0).Item2.Sum() > 0;
            AttachedToBottom = rows.FirstOrDefault(x => x.Item1 == Size - 1).Item2.Sum() > 0;
        }

        public Tuple<int, int> AddDelta(Tuple<int, int> delta)
        {
            return new Tuple<int, int>(Row + delta.Item1, Column + delta.Item2);
        }

        public void AttachTo(ListHex node)
        {
            if (node != null
                && node.Owner == Owner)
                Attached[node.Row, node.Column] = 1;
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
                return Attached[node.Row, node.Column] == 1;
            }

            return false;
        }
    }
}