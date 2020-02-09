using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Engine.GameTypes;
using Engine.Interfaces;
using Engine.Players;

namespace Engine
{
    public class Move
    {
        public IPlayer player;
        public Hex hex;
    }
    public class Referee
    {
        // Board size must be equal in both directions
        public int Size;
        public Board Board;
        private IPlayer _lastPlayer;
        public List<Hex> winningPath;
        public Hex clickedHex;
        public List<Move> AllGameMoves;


        // we need a player 1
        private IPlayer _player1;

        // we need a player 2
        private IPlayer _player2;

        public Hex lastHexForPlayer1;
        public Hex lastHexForPlayer2;

        private EventWaitHandle waitHandle = new AutoResetEvent(false);
        private Hex hexWanted;

        public IPlayer CurrentPlayer()
        {
            return _player1 == _lastPlayer ? _player2 : _player1;
        }

        public IPlayer LastPlayer()
        {
            return _lastPlayer;
        }

        public void SwitchPlayers()
        {
            _lastPlayer = _player1 == _lastPlayer ? _player2 : _player1;
        }
        public void ClickOnHexCoords(int x, int y)
        {
            Board.clickedHex = Board.CheckHex(x, y) ? Board.HexAt(x, y) : null;
        }

        public void CreatePlayer(string type, int playerNum)
        {
            switch (type)
            {
                case "Human":
                {
                    if (playerNum == 1)
                    {
                        _player1 = new HumanPlayer();
                    }
                    else
                    {
                        _player2 = new HumanPlayer();
                    }

                    break;
                }
                   
                default:
                {
                    if (playerNum == 1)
                    {
                        _player1 = new RandomPlayer(1);
                    }
                    else
                    {
                        _player2 = new RandomPlayer(2);
                    }

                    break;
                }
            }
            _player1.PlayerNumber = 1;
            _player2.PlayerNumber = 2;

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
            Board.clickedHex = null;
            AllGameMoves = new List<Move>();
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
        
        public async Task<Hex> TakeTurn(IPlayer player)
        {
            hexWanted = null;

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

            hexWanted = await Task.Run(() =>player.SelectHex(Board));

            if (hexWanted != null)
            {
                Board.TakeHex(hexWanted.X, hexWanted.Y, CurrentPlayer());

                if (CurrentPlayer().PlayerNumber == 1)
                {
                    lastHexForPlayer1 = hexWanted;
                }
                else
                {
                    lastHexForPlayer2 = hexWanted;
                }
                
            }
            var playerMove = new Move();
            playerMove.player = CurrentPlayer();
            playerMove.hex = hexWanted;
            AllGameMoves.Add(playerMove);
            return hexWanted;

        }


        private void PrintPath(List<Hex> path)
        {
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

            foreach (var hex in startingHexes)
            {
                var path = new List<Hex> ();
                if (CheckForWinningPath(path, hex, horizontal))
                {
                    

                    try
                    {
                        var pathmonger = new Pathmonger(Size, horizontal);
                        pathmonger.SetUpAvailableBlocks(Board.Spaces, path);
                        var bestPath = pathmonger.Start();
                        if (bestPath != null)
                        {
                            foreach (var step in bestPath.OrderByDescending(x => x.F))
                            {
                                var tempHex = Board.Spaces.FirstOrDefault(x =>
                                    x.X == step.Location.X && x.Y == step.Location.Y);
                                if (tempHex != null)
                                {
                                    winningPath.Add(tempHex);
                                }
                            }
                            Console.WriteLine("Best path is (" + bestPath.Count() + "): ");
                            foreach (var node in bestPath)
                            {
                                Console.Write("[" + node.Location.X + "," + node.Location.Y + "] ");
                            }
                            Console.WriteLine();
                            Console.WriteLine("-----");
                        } 
                        else
                        {
                            winningPath = path;
                            Console.WriteLine("Best path not found");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Pathmonger pooped.  " + e.Message);
                    }
                    
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
                    Console.Write("Winning path is (" + currentPath.Count() + "): ");
                    PrintPath(currentPath);
                    return true;
                }
            }
            else
            {
                if (currentHex.Y == Size - 1)
                {
                    Console.Write("Winning path is (" + currentPath.Count() + "): ");
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