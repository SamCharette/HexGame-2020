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
            Board.ClickedHex = Board.CheckHex(x, y) ? Board.HexAt(x, y) : null;
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
            Board.ClickedHex = null;
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
                case "Pathfinder AI":
                    if (playerNumber == 1)
                    {
                        Player1 = new PathFinderPlayer(playerNumber, Size);
                    }
                    else
                    {
                        Player2 = new PathFinderPlayer(playerNumber, Size);
                    }

                    break;
                case "Dozer AI":
                    if (playerNumber == 1)
                    {
                        Player1 = new DozerPlayer(playerNumber, Size);
                    }
                    else
                    {
                        Player2 = new DozerPlayer(playerNumber, Size);
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
            
            if (CheckForWinningPath(horizontal))
            {
                WinningPlayer = CurrentPlayer();
                return true;
            }

            SwitchPlayers();
            return false;
        }

     

        private bool CheckForWinningPath(bool isHorizontal)
        {

            Board.FindBestPath(isHorizontal);
            if (Board.BestPath.Any())
            {
                PrintPath(Board.BestPath);
                winningPath = Board.BestPath;
                return true;
            }

            return false;

        }
    }
}