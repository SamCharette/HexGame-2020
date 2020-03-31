using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Players;

namespace NegamaxPlayer
{
    public class Board
    {
        public List<Hex> Hexes { get; set; }
        public int Size { get; set; }

        public void Setup(int size)
        {
            Size = size;
            Hexes = new List<Hex>(Size * Size);
        }

        public bool HasWinner()
        {
            return false;
        }

        public int Score()
        {
            return 0;
        }

        public Hex HexAt(int row, int column)
        {
            return Hexes.FirstOrDefault(x => x.Row == row && x.Column == column);
        }

        public Hex HexAt(Tuple<int,int> coordinates)
        {
            return HexAt(coordinates.Item1, coordinates.Item2);
        }

        public Hex HexAt(BaseNode node)
        {
            return HexAt(node.ToTuple());
        }

        public Board GetCopy()
        {
            throw new NotImplementedException();
        }

        public void TakeHex(Hex node)
        {
            throw new NotImplementedException();
        }
    }
}
