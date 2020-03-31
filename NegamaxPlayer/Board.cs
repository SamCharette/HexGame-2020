using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Players.Common;

namespace NegamaxPlayer
{
    public class Board
    {
        public List<BaseNode> Hexes { get; set; }

        public BaseNode HexAt(int row, int column)
        {
            return Hexes.FirstOrDefault(x => x.Row == row && x.Column == column);
        }

        public BaseNode HexAt(Tuple<int,int> coordinates)
        {
            return HexAt(coordinates.Item1, coordinates.Item2);
        }

        public BaseNode HexAt(BaseNode node)
        {
            return HexAt(node.ToTuple());
        }
    }
}
