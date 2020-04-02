using System;
using System.Collections.Generic;
using System.Text;
using Players;

namespace NegamaxPlayer
{
    public class Hex
    {
        public int Row;
        public int Column;
        public Status Status;
        public int G;
        public int H;
        
        public Guid RandomValue;
        public int Owner;

        public  Hex Parent = null;
        public List<Hex> Neighbours = new List<Hex>();
        private string _hexName;

        public int Size { get; set; }

        public Hex()
        {

        }
        public Hex(Hex original)
        {
            SetValues(original.Size, original.Row, original.Column);
            GetNeighbours();
        }

        
        public Hex(int size, int row, int column)
        {
            SetValues(size, row, column);
        }

        private void SetValues(int size = 11, int row = 0, int column = 0)
        {
            Size = size;
            Row = row;
            Column = column;
            Status = Status.Untested;
            G = 0;
            H = 0;
            Parent = null;
            Owner = 0;
            RandomValue = Guid.NewGuid();
            Neighbours = new List<Hex>();
        }

        public int F()
        {
            return G + H;
        }

        public int DistanceTo(Hex dest)
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

        public bool IsInBounds()
        {
            return Row >= 0 && Row < Size && Column >= 0 && Column < Size;
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

  
        public void GetNeighbours()
        {
            Neighbours = new List<Hex>();
            for (var i = 0; i < 6; i++)
            {
                var coordinates = AddDelta(Compass.GetCoordinatesFor(ToTuple(), i));

                var newNeighbour = new Hex(Size, coordinates.Item1, coordinates.Item2);

                if (newNeighbour.IsInBounds())
                {
                    Neighbours.Add(newNeighbour);
                }
            }
        }
        public Tuple<int, int> AddDelta(Tuple<int, int> delta)
        {
            return new Tuple<int, int>(Row + delta.Item1, Column + delta.Item2);
        }

        public void ClearPathingVariables()
        {
            G = 0;
            H = 0;
            Owner = 0;
            Status = Status.Untested;
            Parent = null;
        }

        public override string ToString()
        {
            return ToTuple().ToString() + "/" + Owner;
        }
    }
}
