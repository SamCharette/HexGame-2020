using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Players.Common;

namespace Players.Minimax.List
{
    public class ListHex
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public List<ListHex> Attached { get; set; }
        public int F => G + H;
        public int G { get; set; }
        public int H { get; set; }
        public Common.PlayerType Owner { get;set; }
        public Status Status { get; set; }
        private string _hexName;

        public string HexName
        {
            get
            {
                if (string.IsNullOrEmpty(_hexName))
                {
                    return "(" + Row + "," + Column + ")";
                }

                return _hexName;
            }
            set
            {
                _hexName = value;
            }
        }

        public ListHex Parent;
        public Guid RandomValue;


        public ListHex(int size, int row, int column)
        {
            Parent = null;
            RandomValue = Guid.NewGuid();
            Status = Status.Untested;
            Attached = new List<ListHex>();
            G = 0;
            H = 0;
            Row = row;
            Column = column;
            
            Owner = PlayerType.White;
        }

        public Tuple<int, int> ToTuple()
        {
            return new Tuple<int, int>(Row, Column);
        }
        public override string ToString()
        {
            return ToTuple().ToString();
        }
        public ListHex(Tuple<int,int> tupleData)
        {
            Row = tupleData.Item1;
            Column = tupleData.Item2;
        }
        public bool Equals(ListHex other)
        {
            if (Row == other.Row && Column == other.Column)
            {
                return true;
            }
            return false;
        }
        public bool Equals(Tuple<int,int> coordinates)
        {
            if (Row == coordinates.Item1 && Column == coordinates.Item2)
            {
                return true;
            }
            return false;
        }
        public Tuple<int,int> AddDelta(Tuple<int,int> delta)
        {
            return new Tuple<int, int>(Row + delta.Item1, Column + delta.Item2);
        }
        public void ClearPathingVariables()
        {
            G = 0;
            H = 0;
            Parent = null;
            var toDetach = Attached.Where(x => x.Owner != Owner).ToList();
            toDetach.ForEach(DetachFrom);


            Status = Status.Untested;
        }

        public void AttachTo(ListHex node)
        {
            if (node != null && !IsAttachedTo(node) && !Equals(node) && node.Owner == Owner)
            {
                Attached.Add(node);
            }
        }

        public void DetachFrom(ListHex node)
        {
            if (IsAttachedTo(node))
            {
                var beforeCount = Attached.Count();
                Attached.Remove(node);
                if (Attached.Count == beforeCount)
                {
                    Console.WriteLine("Detachment failed: " + node.HexName + "-" + HexName);
                }
            }
        }
        public bool IsAttachedTo(ListHex node)
        {
            if (node != null) 
            {
                var nodeToCheck = Attached.FirstOrDefault(x => Equals(node));
                if (nodeToCheck != null)
                {
                    return true;
                }
            }

            return false;
        }
    }

}
