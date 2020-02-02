using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Engine.GameTypes;
using Engine.Interfaces;

namespace Engine
{
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

        public List<Hex> GetFriendlyNeighbours(float x, float y, IPlayer player)
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

        public List<Hex> GetNeighbours(float x, float y)
        {
            var neighbours = new List<Hex>();
            // Top right
            if (CheckHex(x+1, y-1))
            {
                neighbours.Add(HexAt(x+1,y-1));
            }
            // Right
            if (CheckHex(x + 1, y))
            {
                neighbours.Add(HexAt(x + 1, y));
            }
            // Bottom right
            if (CheckHex(x , y + 1))
            {
                neighbours.Add(HexAt(x, y + 1));
            }
            // Bottom Left
            if (CheckHex(x - 1, y + 1))
            {
                neighbours.Add(HexAt(x - 1, y + 1));
            }
            // Left
            if (CheckHex(x - 1, y ))
            {
                neighbours.Add(HexAt(x - 1, y ));
            }
            // Top Left
            if (CheckHex(x , y -1))
            {
                neighbours.Add(HexAt(x , y -1));
            }


            return neighbours;
        }

        public Hex HexAt(float x, float y)
        {
            return Spaces.FirstOrDefault(hex => hex.X == x && hex.Y == y);
        }
        public bool TakeHex(float x, float y, IPlayer player)
        {
            var hexToTake = HexAt(x, y);
            if (hexToTake != null && hexToTake.Owner == null)
            {
                hexToTake.Owner = player;
                return true;
            }
            return false;
        }

        public bool CheckHex(float x, float y)
        {
            return HexAt(x, y) != null;
        }

        public bool CheckHexForPlayer(int x, int y, IPlayer player)
        {
            var hex = HexAt(x, y);
            return hex?.Owner != null && hex.Owner.PlayerNumber == player.PlayerNumber;
        }

        public IPlayer Winner(IPlayer player)
        {
            if (player == null)
            {
                return null;
            }

            var horizontal = player.PlayerNumber == 1;
            // Let's go through the hexes on the 0 side of the appropriate player,
            // and start a depth-first search for a connection to the other side.
            List<Hex> startingHexes;

            if (horizontal)
            {
                startingHexes = Spaces.Where(x => x.Y == 0 && x.Owner?.PlayerNumber == 1).ToList();
            }
            else
            {
                startingHexes = Spaces.Where(x => x.X == 0 && x.Owner?.PlayerNumber == 2).ToList();
            }

            foreach (var hex in startingHexes)
            {
                var path = new List<Hex> {hex};
                if (CheckForWinningPath(path, hex, horizontal))
                {
                    return player;
                }
            }

            return null;
        }

        private bool CheckForWinningPath(List<Hex> currentPath, Hex currentHex, bool isHorizontal)
        {
            if (isHorizontal)
            {
                if ((int)Math.Round(currentHex.Y) == _size - 1)
                {
                    return true;
                }
            }
            else
            {
                if ((int) Math.Round(currentHex.Y) == _size - 1)
                {
                    return true;
                }
            }

            currentPath.Add(currentHex);
            var friendlyNeighboursNotLookedAtAlready =
                GetFriendlyNeighbours(currentHex.X, currentHex.Y, currentHex.Owner)
                    .Where(x => currentPath.Count(y => y.X == x.X && y.Y == x.Y) < 1).ToList();
            foreach (var hex in friendlyNeighboursNotLookedAtAlready)
            {
                if (CheckForWinningPath(currentPath, hex, isHorizontal))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
