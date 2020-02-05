using System.Collections.Generic;
using System.Linq;
using Engine.GameTypes;
using Engine.Interfaces;

namespace Engine
{
    public enum Direction
    {
        TopRight,
        Right,
        BottomRight,
        BottomLeft,
        Left,
        TopLeft
    }
    public class Board
    {
        public List<Hex> Spaces;
        private int _size;

        public Board(int size = 11)
        {
            _size = size;
            Spaces = new List<Hex>();
            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    var hex = new Hex(i, j);
                    Spaces.Add(hex);
                }
            }
        }

        public List<Hex> GetFriendlyNeighbours(int x, int y, IPlayer player)
        {
            var allNeighbours = GetNeighbours(x, y);
            return allNeighbours?.Where(hex => hex.Owner != null && hex.Owner.PlayerNumber == player.PlayerNumber)
                .ToList();
        }

        public List<Hex> GetEnemyNeighbours(int x, int y, IPlayer player)
        {
            var allNeighbours = GetNeighbours(x, y);
            return allNeighbours?.Where(hex => hex.Owner != null && hex.Owner.PlayerNumber != player.PlayerNumber)
                .ToList();
        }

        public Hex GetNeighbourAt(Direction direction, int x, int y)
        {
            switch (direction)
            {
                case Direction.TopRight:
                    return HexAt(x + 1, y - 1);
                case Direction.Right:
                    return HexAt(x + 1, y);
                case Direction.BottomRight:
                    return HexAt(x , y + 1);
                case Direction.BottomLeft:
                    return HexAt(x - 1, y + 1);
                case Direction.Left:
                    return HexAt(x - 1, y );
                case Direction.TopLeft:
                    return HexAt(x , y - 1);

                default:
                    return null;
            }

        }

        public List<Hex> GetNeighbours(int x, int y)
        {
            var neighbours = new List<Hex>
            {
                GetNeighbourAt(Direction.TopRight, x, y),
                GetNeighbourAt(Direction.Right, x, y),
                GetNeighbourAt(Direction.BottomRight, x, y),
                GetNeighbourAt(Direction.BottomLeft, x, y),
                GetNeighbourAt(Direction.Left, x, y),
                GetNeighbourAt(Direction.TopLeft, x, y)
            };


            return neighbours.Where(hex => hex != null).ToList();
        }

        public Hex HexAt(int x, int y)
        {
            return Spaces.FirstOrDefault(hex => hex.X == x && hex.Y == y);
        }

        public bool TakeHex(int x, int y, IPlayer player)
        {
            var hexToTake = HexAt(x, y);
            if (hexToTake != null && hexToTake.Owner == null)
            {
                hexToTake.Owner = player;
                return true;
            }

            return false;
        }

        public bool CheckHex(int x, int y)
        {
            return HexAt(x, y) != null;
        }

        public bool CheckHexForPlayer(int x, int y, IPlayer player)
        {
            var hex = HexAt(x, y);
            return hex?.Owner != null && hex.Owner.PlayerNumber == player.PlayerNumber;
        }

    }
}
