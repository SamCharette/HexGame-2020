using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Omu.ValueInjecter;
using Players;
using Console = System.Console;

namespace NegamaxPlayer
{
    public class Board
    {
        public List<Hex> Hexes { get; set; }
        public int Size { get; set; }
        private int CopyNumber = 0;

        public Board()
        {

        }
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

        public Board(Board original)
        {
            Size = original.Size;
            Hexes = new List<Hex>(Size * Size);
            foreach (var originalHex in original.Hexes)
            {
                var hex = new Hex(originalHex);
                Hexes.Add(hex);
            }

            CopyNumber = original.CopyNumber + 1;
        }

        public bool HasWinner()
        {

            var scout = new Pathfinder(this, 1);
            var path = scout.GetBestPathForPlayer();
            var player1Winner = path.All(x => x.Owner == 1);

            if (!player1Winner)
            {
                scout.SetPlayer(-1);
                path = scout.GetBestPathForPlayer();
                var player2Winner = path.All(x => x.Owner == -1);
                if (!player2Winner)
                {
                    return false;
                }
            }
          
            return true;
    
            //// Check for player 1
            //var player1Win = false;
            //var startSpots = Hexes.Where(x => x.Row == 0 && x.Owner == 1).ToList();
            //var endSpots = Hexes.Where(x => x.Row == Size - 1 && x.Owner == 1).ToList();
            //if (startSpots.Any() && endSpots.Any())
            //{
            //    player1Win = IsWinningPathBetween(startSpots, endSpots);
            //}

            //// Check for player 2
            //if (!player1Win)
            //{
            //    startSpots = Hexes.Where(x => x.Column == 0 && x.Owner == -1).ToList();
            //    endSpots = Hexes.Where(x => x.Column == Size - 1 && x.Owner == -1).ToList();
            //    if (startSpots.Any() && endSpots.Any())
            //    {
            //        return IsWinningPathBetween(startSpots, endSpots);
            //    }
            //}

            //return true;
        }


        public string GetHash()
        {
            var code = "";
            foreach (var hex in Hexes.OrderBy(x => x.Row).ThenBy(x => x.Column))
            {
                code = code + "[" + hex.Row + hex.Column + hex.Owner + "]";
            }
            return code;
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

        public Hex HexAt(Hex node)
        {
            return HexAt(node.ToTuple());
        }

        public void TakeHex(Tuple<int,int> coordinates, int playerNumber)
        {
            HexAt(coordinates).Owner = playerNumber;
        }

        public List<Hex> GetNeighboursFrom(Hex hex, int player)
        {
            var opponent = player == 1 ? -1 : 1;
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
