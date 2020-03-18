using System;
using System.Collections.Concurrent;
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
        public ConcurrentDictionary<string, ListHex> Attached { get; set; }
        public int F => G + H;
        public int G { get; set; }
        public int H { get; set; }
        public Common.PlayerType Owner { get;set; }
        public Status Status { get; set; }
        public ListHex Parent;
        public Guid RandomValue;

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


        public ListHex()
        {
            Parent = null;
            RandomValue = Guid.NewGuid();
            Status = Status.Untested;
            Attached = new ConcurrentDictionary<string, ListHex>();
            G = 0;
            H = 0;

            Owner = PlayerType.White;
        }

        public ListHex(int size, int row, int column)
        {
            Parent = null;
            RandomValue = Guid.NewGuid();
            Status = Status.Untested;
            Attached = new ConcurrentDictionary<string, ListHex>();
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
            if (ToTuple().Equals(other.ToTuple()))
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
            Attached = new ConcurrentDictionary<string, ListHex>();
            Status = Status.Untested;
        }

        public void AttachTo(ListHex node)
        {
            if (node != null
                && node.Owner == Owner)
            {
                Attached.TryAdd(node.HexName, node);
            }
        }

        public void DetachFrom(ListHex node)
        {
            if (node != null)
            {
                ListHex outHex;
                Attached.TryRemove(node.HexName, out outHex);
            }
        }
        public bool IsAttachedTo(ListHex node)
        {
            if (node != null)
            {
                var nodeToCheck = Attached[node.HexName];
                if (nodeToCheck != null)
                {
                    return true;
                }
            }

            return false;
        }
    }

}
