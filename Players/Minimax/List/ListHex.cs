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
            if (node != null && !IsAttachedTo(node) && node.Owner == Owner)
            {
                Attached.Add(node);
            }
        }

        public void DetachFrom(ListHex node)
        {
            if (IsAttachedTo(node))
            {
                Attached.Remove(node);
            }
        }
        public bool IsAttachedTo(ListHex node)
        {
            return node != null && Attached.Contains(node);
        }
    }

}
