using System;
using System.Collections.Generic;
using System.Linq;
using Engine.GameTypes;
using Engine.Interfaces;
using Engine.Players;

namespace Engine
{
    public class Referee
    {
        // Board size must be equal in both directions
        public int Size;
        public Board Board;
        private IPlayer _lastPlayer;
        public List<Hex> winningPath;

        // we need a player 1
        private IPlayer _player1;

        // we need a player 2
        private IPlayer _player2;

        public Hex lastHexForPlayer1;
        public Hex lastHexForPlayer2;

        public IPlayer CurrentPlayer()
        {
            return _player1 == _lastPlayer ? _player2 : _player1;
        }

        public IPlayer LastPlayer()
        {
            return _lastPlayer;
        }

        void SwitchPlayers()
        {
            _lastPlayer = _player1 == _lastPlayer ? _player2 : _player1;
        }


        // We need a way to get input from a player

        // We need a way to send the new board and game state to a player


        // functions
        public Referee(int size = 11)
        {
            NewGame(size);
            AddPlayer(new RandomPlayer(1), 1);
            AddPlayer(new RandomPlayer(2), 2);
            
        }

        public void NewGame(int size = 11)
        {
            Size = size;
            Board = new Board(size);
            winningPath = new List<Hex>();
        }

        public void AddPlayer(IPlayer player, int playerNumber)
        {
            if (playerNumber == 1)
            {
                _player1 = player;
            }
            else
            {
                _player2 = player;
            }
        }

        public Board Play()
        {
            try
            {
                while (Winner() == false)
                {
                    Console.WriteLine("Player taking turn: " + CurrentPlayer().PlayerNumber);
                    var hexTaken = TakeTurn(CurrentPlayer());
                    if (hexTaken != null)
                    {
                        Console.WriteLine("Hex selected was : " + hexTaken.X + ", " + hexTaken.Y);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("No winner today!");

            }

            return Board;

        }

        public Hex TakeTurn(IPlayer player)
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

            var hexWanted = player.SelectHex(Board);
            Board.TakeHex(hexWanted.X, hexWanted.Y, CurrentPlayer());

            if (CurrentPlayer().PlayerNumber == 1)
            {
                lastHexForPlayer1 = hexWanted;
            }
            else
            {
                lastHexForPlayer2 = hexWanted;
            }

            return hexWanted;

        }

        private void PrintPath(List<Hex> path)
        {
            Console.Write("Path is: ");
            foreach (var hex in path)
            {
                Console.Write("[" + hex.X + "," + hex.Y + "] ");
            }
            Console.WriteLine("");
        }

        public bool Winner()
        {
     

            var horizontal = CurrentPlayer().PlayerNumber == 1;
            // Let's go through the hexes on the 0 side of the appropriate player,
            // and start a depth-first search for a connection to the other side.
            List<Hex> startingHexes;

            if (horizontal)
            {
                startingHexes = Board.Spaces.Where(x => x.X == 0 && x.Owner?.PlayerNumber == 1).ToList();
            }
            else
            {
                startingHexes = Board.Spaces.Where(x => x.Y == 0 && x.Owner?.PlayerNumber == 2).ToList();
            }
            PrintPath(startingHexes);

            foreach (var hex in startingHexes)
            {
                var path = new List<Hex> ();
                if (CheckForWinningPath(path, hex, horizontal))
                {
                    winningPath = path;
                    return true;
                }
            }
            SwitchPlayers();
            return false;
        }

        private bool CheckForWinningPath(List<Hex> currentPath, Hex currentHex, bool isHorizontal)
        {

            currentPath.Add(currentHex);
            if (isHorizontal)
            {
                if (currentHex.X == Size - 1)
                {
                    Console.Write("Winning path is : ");
                    PrintPath(currentPath);
                    return true;
                }
            }
            else
            {
                if (currentHex.Y == Size - 1)
                {
                    Console.Write("Winning path is : ");
                    PrintPath(currentPath);
                    return true;
                }
            }

            var friendlyNeighboursNotLookedAtAlready =
                Board.GetFriendlyNeighbours(currentHex.X, currentHex.Y, currentHex.Owner)
                    .Where(x => !currentPath.Any(y => y.X == x.X && y.Y == x.Y)).ToList();

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