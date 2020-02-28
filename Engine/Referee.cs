using System;
using System.Collections.Generic;

using System.Threading;
using System.Threading.Tasks;
using HexLibrary;
using Players;
using Players.Base;
using Players.Common;
using Players.Minimax;

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
        public Map Board;
        private Player _lastPlayer;
        private Tuple<int, int> _lastPlay;
        public List<Hex> winningPath;
        public Player WinningPlayer;
        public Hex clickedHex;
        public List<Move> AllGameMoves;

        public Player Player1 { get; set; }
        public Player Player2 { get; set; }


        public Tuple<int,int> lastHexForPlayer1;
        public Tuple<int,int> lastHexForPlayer2;

        private EventWaitHandle waitHandle = new AutoResetEvent(false);
        private Tuple<int,int> hexWanted;

        public Player CurrentPlayer()
        {
            return Player1 == _lastPlayer ? Player2 : Player1;
        }

        public Player OpponentPlayer()
        {
            return Player1 == _lastPlayer ? Player1 : Player2;
        }

        public void Quip(string expressionToSay)
        {
            Console.WriteLine("Referee: " + expressionToSay);
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
        }

        public Referee(int size = 11)
        {
            NewGame(size);
            AddPlayer(new Config(), 1);
            AddPlayer(new Config(), 2);
            
        }

        public void NewGame(int size = 11)
        {
            Size = size;
            Board = new Map(size);
            winningPath = new List<Hex>();
            WinningPlayer = null;
            AllGameMoves = new List<Move>();
            _lastPlayer = Player2;
        }

        public void AddPlayer(Config playerConfig, int playerNumber)
        {
        
            switch (playerConfig.type)
            {
                case "Human":
                    if (playerNumber == 1)
                    {
                        Player1 = new HumanPlayer(playerNumber, Size, playerConfig);
                    }
                    else
                    {
                        Player2 = new HumanPlayer(playerNumber, Size, playerConfig);
                    }
                   
                    break;
                case "Dozer AI":
                    if (playerNumber == 1)
                    {
                        Player1 = new DozerPlayer(playerNumber, Size, playerConfig);
                    }
                    else
                    {
                        Player2 = new DozerPlayer(playerNumber, Size, playerConfig);
                    }

                    break;
                case "Minimax AI":
                    if (playerNumber == 1)
                    {
                        Player1 = new MinimaxPlayer(playerNumber, Size, playerConfig);
                    }
                    else
                    {
                        Player2 = new MinimaxPlayer(playerNumber, Size, playerConfig);
                    }

                    break;

                case "Replay AI":
                    if (playerNumber == 1)
                    {
                        Player1 = new Playback(playerNumber, Size, playerConfig);
                    }
                    else
                    {
                        Player2 = new Playback(playerNumber, Size, playerConfig);
                    }

                    break;

                default:
                    if (playerNumber == 1)
                    {
                        Player1 = new RandomPlayer(playerNumber, Size, null);
                    }
                    else
                    {
                        Player2 = new RandomPlayer(playerNumber, Size, null);
                    }
                    break;
            }
           
        }
        
        public async Task<Tuple<int,int>> TakeTurn(Player player)
        {
            Quip(player.PlayerType() + " take your turn!");
            hexWanted = null;

            // First, check to see if the player is empty
            if (player == null)
            {
                Quip("Non player trying to take a turn?");
                throw new Exception("Cannot take a turn as a non-player");
            }

            // Next, check to see if the player is the same as the last one
            if (player == _lastPlayer)
            {
                Quip("FOUL!  Taking a second turn!");
                WinningPlayer = (player == Player1 ? Player2 : Player1);
                throw new Exception("Cannot play twice in a row");
            }

            hexWanted = await Task.Run(() => player.SelectHex(_lastPlay));

            if (hexWanted == null)
            {
                Quip("FOUL!  No hex was selected.  Player LOSES.");
                WinningPlayer = LastPlayer();
                return null;
            }
            else
            {
                var success = Board.TakeHex(new Hex(hexWanted.Item1, hexWanted.Item2), CurrentPlayer().PlayerNumber);

                if (!success)
                {
                    Quip("FOUL!  Player tried to take a hex that was blocked!");
                    WinningPlayer = (player == Player1 ? Player2 : Player1);
                    throw new Exception("Can't pick a blocked hex");
                }

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

        public void Dispose()
        {
            Size = 0;
            Board = null;
            Player1.GameOver(WinningPlayer.PlayerNumber);
            Player2.GameOver(WinningPlayer.PlayerNumber);
            winningPath = null;
            WinningPlayer = null;
            _lastPlayer = null;
        }

        private void PrintPath(List<Hex> path)
        {
            foreach (var hex in path)
            {
                Console.Write("[" + hex.Row + "," + hex.Column + "] ");
            }
            Console.WriteLine("");
        }

        public bool LookForWinner()
        {

            if (WinningPlayer != null)
            {
                return true;
            }

            var isHorizontal = CurrentPlayer().PlayerNumber != 1;
            
            if (CheckForWinningPath(CurrentPlayer().PlayerNumber))
            {
                WinningPlayer = CurrentPlayer();
                Quip("The winner is player #" + WinningPlayer.PlayerNumber + ", " + WinningPlayer.PlayerType() + "!");
                return true;
            }

            SwitchPlayers();
            return false;
        }

     

        private bool CheckForWinningPath(int playerNumber)
        {

            if (Board.DoesWinningPathExistFor(playerNumber))
            {
                winningPath = Board.LastPathChecked;
                return true;
            }

            return false;

        }
    }
}