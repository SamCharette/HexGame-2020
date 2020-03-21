using System;
using System.Collections.Generic;
using System.Text;

namespace Players.Minimax.List
{
    public class SimpleHex
    {
        private string _hexName;
        public int Row { get; set; }
        public int Column { get; set; }
        public int Size { get; set; }

        public SimpleHex(Tuple<int,int> coordinates)
        {
            Row = coordinates.Item1;
            Column = coordinates.Item2;
        }

        public SimpleHex(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public bool IsInBounds()
        {
            return Row > 0 && Row < Size && Column > 0 && Column < Size;
        }

        public string HexName
        {
            get
            {
                if (string.IsNullOrEmpty(_hexName)) return "(" + Row + "," + Column + ")";

                return _hexName;
            }
            set => _hexName = value;
        }

        public Tuple<int, int> ToTuple()
        {
            return new Tuple<int, int>(Row, Column);
        }

        public override string ToString()
        {
            return ToTuple().ToString();
        }

        public bool Equals(ListHex other)
        {
            if (ToTuple().Equals(other.ToTuple())) return true;
            return false;
        }

    }
}
