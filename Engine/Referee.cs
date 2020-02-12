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
        public Player player;
        public Tuple<int,int> hex;
    }
    public class Referee
    {
        // Board size must be equal in both directions
        public int Size;
        public Board Board;
        private Player _lastPlayer;
        private Tuple<int, int> _lastPlay;
        public List<Hex> winningPath;
        public Player WinningPlayer;
        public Hex clickedHex;
        public List<Move> AllGameMoves;

        public Player Player1 { get; private set; }
        public Player Player2 { get; private set; }


        public Tuple<int,int> lastHexForPlayer1;
        public Tuple<int,int> lastHexForPlayer2;

        private EventWaitHandle waitHandle = new AutoResetEvent(false);
        private Tuple<int,int> hexWanted;

        public Player CurrentPlayer()
        {
            return Player1 == _lastPlayer ? Player2 : Player1;
        }

      

        public Player LastPlayer()
        {
            return _lastPlayer;
        }

        public void SwitchPlayers()
        {
            _lastPlayer = Player1 == _lastPlayer ? Player2 : Player1;
        }
        public void ClickOnHexCoords(int x, int y)
        {
            if (CurrentPlayer() is HumanPlayer)
            {
                HumanPlayer player = (HumanPlayer)CurrentPlayer();
                player.ClickMadeOn(new Tuple<int, int>(x, y));
            }
            Board.clickedHex = Board.CheckHex(x, y) ? Board.HexAt(x, y) : null;
        }

       
        // We need a way to get input from a player

        // We need a way to send the new board and game state to a player


        // functions
        public Referee(int size = 11)
        {
            NewGame(size);
            AddPlayer("", 1);
            AddPlayer("", 2);
            
        }

        public void NewGame(int size = 11)
        {
            Size = size;
            Board = new Board(size);
            winningPath = new List<Hex>();
            WinningPlayer = null;
            Board.clickedHex = null;
            AllGameMoves = new List<Move>();
            _lastPlayer = Player2;
        }

        public void AddPlayer(string playerType, int playerNumber)
        {
        
            switch (playerType)
            {
                case "Human":
                    if (playerNumber == 1)
                    {
                        Player1 = new HumanPlayer(playerNumber, Size);
                    }
                    else
                    {
                        Player2 = new HumanPlayer(playerNumber, Size);
                    }
                   
                    break;
                default:
                    if (playerNumber == 1)
                    {
                        Player1 = new RandomPlayer(playerNumber, Size);
                    }
                    else
                    {
                        Player2 = new RandomPlayer(playerNumber, Size);
                    }
                    break;
            }
           
        }
        
        public async Task<Tuple<int,int>> TakeTurn(Player player)
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

            hexWanted = await Task.Run(() => player.SelectHex(_lastPlay));

            if (hexWanted == null)
            {
                Console.WriteLine("Referee calls foul!  No hex was selected.  Player LOSES.");
                WinningPlayer = LastPlayer();
                return null;
            }
            else
            {
                Board.TakeHex(hexWanted.Item1, hexWanted.Item2, CurrentPlayer().PlayerNumber);

                if (CurrentPlayer().PlayerNumber == 1)
                {
                    lastHexForPlayer1 = hexWanted;
                }
                else
                {
                    lastHexForPlayer2 = hexWanted;
                }

                _lastPlay = hexWanted; 
                var playerMove = new Move();
                playerMove.player = CurrentPlayer();
                playerMove.hex = hexWanted;
                AllGameMoves.Add(playerMove);
                LookForWinner();
                return new Tuple<int, int>(hexWanted.Item1, hexWanted.Item2);
            }
           

        }


        private void PrintPath(List<Hex> path)
        {
            foreach (var hex in path)
            {
                Console.Write("[" + hex.X + "," + hex.Y + "] ");
            }
            Console.WriteLine("");
        }

        public bool LookForWinner()
        {

            if (WinningPlayer != null)
            {
                return true;
            }

            var horizontal = CurrentPlayer().PlayerNumber != 1;
            // Let's go through the hexes on the 0 side of the appropriate player,
            // and start a depth-first search for a connection to the other side.
            List<Hex> startingHexes;

            if (!horizontal)
            {
                startingHexes = Board.Spaces.Where(x => x.X == 0 && x.Owner == 1).ToList();
            }
            else
            {
                startingHexes = Board.Spaces.Where(x => x.Y == 0 && x.Owner == 2).ToList();
            }

            foreach (var hex in startingHexes)
            {
                var path = new List<Hex> ();
                if (CheckForWinningPath(path, hex, horizontal))
                {
                    

                    //try
                    //{
                    //    var pathmonger = new Pathmonger(Size, horizontal);
                    //    pathmonger.SetUpAvailableBlocks(Board.Spaces);
                    //    pathmonger.Start();
                    //    if (pathmonger.FinalPath.Any())
                    //    {
                    //        foreach (var step in pathmonger.FinalPath.OrderByDescending(x => x.F))
                    //        {
                    //            var tempHex = Board.Spaces.FirstOrDefault(x =>
                    //                x.X == step.Location.X && x.Y == step.Location.Y);
                    //            if (tempHex != null)
                    //            {
                    //                winningPath.Add(tempHex);
                    //            }
                    //        }
                    //        Console.WriteLine("Best path is (" + pathmonger.FinalPath.Count() + "): ");
                    //        foreach (var node in pathmonger.FinalPath)
                    //        {
                    //            Console.Write("[" + node.Location.X + "," + node.Location.Y + "] ");
                    //        }
                    //        Console.WriteLine();
                    //        Console.WriteLine("-----");
                    //    } 
                    //    else
                    //    {
                    //        winningPath = path;
                    //        Console.WriteLine("Best path not found");
                    //    }
                    //}
                    //catch (Exception e)
                    //{
                    //    Console.WriteLine("Pathmonger pooped.  " + e.Message);
                    //}

                    WinningPlayer = CurrentPlayer();
                    return true;
                }
            }
            SwitchPlayers();
            return false;
        }

     

        private bool CheckForWinningPath(List<Hex> currentPath, Hex currentHex, bool isHorizontal)
        {

            currentPath.Add(currentHex);
            if (!isHorizontal)
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
                    foreach (var node in currentPath)
                    {
                        winningPath.Add(node);
                    }
                    return true;
                }
            }

            return false;
        }
    }
}