using System;
using System.Collections.Generic;
using System.Linq;

namespace Engine
{
    public class Engine
    {
        // Board size must be equal in both directions
        public int Size;
        public List<Hex> Board;
        private HexOwner _lastPlayer;

        // we need a player 1

        // we need a player 2

        // We need a way to get input from a player

        // We need a way to send the new board and game state to a player


        // functions
        public Engine(int size = 11)
        {
            Size = size;
            _lastPlayer = HexOwner.Player1;
            NewBoard();
        }

        public void NewGame(int size = 11)
        {
            Size = size;
            NewBoard();
            _lastPlayer = HexOwner.Empty;
        }

        public void TakeTurn(HexOwner player, int x, int y)
        {
            if (player == _lastPlayer)
            {
                throw new Exception("Cannot play twice in a row");
            }

            if (CheckHex(x, y))
            {
                AssignHex(x, y, player);
                _lastPlayer = player;
            }

        }

        public bool CheckHex(int x, int y)
        {
            var hexToCheck = Board?.First(hex => hex.X == x && hex.Y == y);

            if (hexToCheck == null) return false;

            return hexToCheck.Owner == HexOwner.Empty;
        }

        private void AssignHex(int x, int y, HexOwner owner)
        {
            var hexToClaim = Board?.First(hex => hex.X == x && hex.Y == y);

            if (hexToClaim != null)
            {
                if (hexToClaim.Owner == HexOwner.Empty)
                {
                    hexToClaim.Owner = owner;
                }
                else
                {
                    throw new Exception("Hex is already taken");
                }
            }
            else
            {
                throw new Exception("Could not find hex to claim");
            }
        }

        public void NewBoard()
        {
            Board = new List<Hex>();

            for (var i = 0; i < Size; i++)
            {
                for (var j = 0; j < Size; j++)
                {
                    var hex = new Hex(i, j);
                    Board.Add(hex);
                }
            }
        }
    }
}