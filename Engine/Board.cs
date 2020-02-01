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

        public Board(int size = 11)
        {
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

        public List<Hex> GetNeighbours(int x, int y)
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
            if (HexAt(x, y) != null)
            {
                return true;
            }
            return false;
        }

        public bool CheckHexForPlayer(int x, int y, IPlayer player)
        {
            var hex = HexAt(x, y);
            if (hex?.Owner != null && hex.Owner.PlayerNumber == player.PlayerNumber)
            {
                return true;
            }

            return false;
        }

        public IPlayer Winner()
        {
            return null;
        }

    }
}
