using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Omu.ValueInjecter;
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
            for (var column = 0; column < size; column++)
            {
                for (var row = 0; row < size; row++)
                {
                    var newHex = new Hex(size, row, column);
                    newHex.GetNeighbours();
                    Hexes.Add(newHex);
                }
            }
        }

        public bool HasWinner()
        {
            return false;
        }

        public int Score(int player)
        {
            var appraiser = new Appraiser();
            var score = appraiser.ScoreFromBoard(this, player);
            return score;
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
            var newBoard = new Board();
            newBoard.Setup(Size);
            newBoard.InjectFrom(this);
            newBoard.Hexes.ForEach(x => x.PostCloneWork());
            return newBoard;
        }

        public void TakeHex(Tuple<int,int> coordinates, int playerNumber)
        {
            HexAt(coordinates).Owner = playerNumber;
        }

        public List<Hex> GetNeighboursFrom(Hex hex, int player)
        {
            var opponent = player == 1 ? 2 : 1;
            var neighbourHexes = hex.Neighbours.ToList();
            var neighbours = neighbourHexes.Select(x => HexAt(x.ToTuple())).ToList();
            neighbours.RemoveAll(x => x.Owner == opponent);
            return neighbours.ToList();

        }

        private List<Hex> GetNeighboursFor(Hex source)
        {
            var neighbours = new List<Hex>();
            foreach (var neighbour in source.Neighbours)
            {
                var neighbourOnBoard = HexAt(neighbour.Row, neighbour.Column);
                if (neighbourOnBoard != null)
                {
                    neighbours.Add(neighbourOnBoard);
                }
            }

            return neighbours.ToList();
        }
    }
}
