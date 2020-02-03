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


            // top right
            if (CheckHex(x + 1, y - 1))
            {
                neighbours.Add(HexAt(x + 1, y - 1));
            }

            //  right
            if (CheckHex(x + 1, y))
            {
                neighbours.Add(HexAt(x + 1, y));
            }

            // bottom right
            if (CheckHex(x, y + 1))
            {
                neighbours.Add(HexAt(x, y + 1));
            }


            // bottom left
            if (CheckHex(x - 1, y + 1))
            {
                neighbours.Add(HexAt(x - 1, y + 1));
            }

            //  left
            if (CheckHex(x - 1, y))
            {
                neighbours.Add(HexAt(x - 1, y));
            }

            // top left
            if (CheckHex(x , y - 1))
            {
                neighbours.Add(HexAt(x , y -1 ));
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

    }
}
