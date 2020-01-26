using System;
using System.Collections.Generic;
using System.Linq;
using Engine.GameTypes;
using Engine.Interfaces;

namespace Engine
{
    public class Referee
    {
        // Board size must be equal in both directions
        public int Size;
        public Board Board;
        private IPlayer _lastPlayer;

        // we need a player 1

        // we need a player 2

        // We need a way to get input from a player

        // We need a way to send the new board and game state to a player


        // functions
        public Referee(int size = 11)
        {
            Size = size;
            NewBoard();
        }

        public void NewGame(int size = 11)
        {
            Size = size;
            NewBoard();
        }

        public void TakeTurn(IPlayer player, int x, int y)
        {
            try
            {
                // First, check to see if the player is empty
                if (player == null)
                {
                    throw new Exception("Cannot take a turn as a non-player");
                }

                // Next, check to see if the player is the same as the last one
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
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool CheckHex(int x, int y)
        {
            if (Board != null && Board.Spaces.Any())
            {
                var hexToCheck = Board.Spaces.FirstOrDefault(hex => hex.X == x && hex.Y == y);

                if (hexToCheck == null)
                {
                    return false;
                }

                return hexToCheck.Owner == null;
            }

            throw new Exception("Can't find a board");
        }

        private void AssignHex(int x, int y, IPlayer owner)
        {
            if (owner == null)
            {
                throw new Exception("Cannot claim hex for non-player");
            }

            var hexToClaim = Board.Spaces?.First(hex => hex.X == x && hex.Y == y);

            if (hexToClaim != null)
            {
                if (hexToClaim.Owner == null)
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
            Board = new Board();

        }
    }
}