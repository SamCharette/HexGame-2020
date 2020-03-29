using System;

namespace MinimaxPlayer.List
{
    public class SimpleHex
    {
        private string _hexName;
        public int Row { get; set; }
        public int Column { get; set; }
        public int Size { get; set; }

        public SimpleHex()
        {

        }
        public SimpleHex(int size, Tuple<int,int> coordinates)
        {
            Size = size;
            Row = coordinates.Item1;
            Column = coordinates.Item2;
        }

        public SimpleHex(int size, int row, int column)
        {
            Size = size;
            Row = row;
            Column = column;
        }

        public bool IsInBounds()
        {
            return Row >= 0 && Row < Size && Column >= 0 && Column < Size;
        }

        public int DistanceTo(SimpleHex dest)
        {
            if (dest == null)
            {
                return 0;
            }
            if (Row == dest.Row)
            {
                return Math.Abs(dest.Column - Column);
            }
            else if (Column == dest.Column)
            {
                return Math.Abs(dest.Row - Row);
            }
            else
            {
                var dx = Math.Abs(dest.Row - Row);
                var dy = Math.Abs(dest.Column - Column);
                if (Column < dest.Column)
                {
                    return dx + dy - (int)(Math.Ceiling(dx / 2.0));
                }
                else
                {
                    return dx + dy - (int)(Math.Floor(dx / 2.0));
                }
            }
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
            return ToTuple().Equals(other.ToTuple());
        }

    }
}
