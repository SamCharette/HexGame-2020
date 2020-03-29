using HexLibrary;
using MinimaxPlayer.List;
using MinimaxPlayer.Matrix;
using PlaybackPlayer;
using Players;
using Players.Base;
using Players.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Engine
{
    public class Move
    {
        public Tuple<int, int> hex;
        public Player player;
    }

    public class Referee
    {
        private Tuple<int, int> _lastPlay;
        private Player _lastPlayer;
        public List<Move> AllGameMoves;
        public Map Board;

        private Tuple<int, int> hexWanted;


        public Tuple<int, int> lastHexForPlayer1;

        public Tuple<int, int> lastHexForPlayer2;

        // Board size must be equal in both directions
        public int Size;
        public List<Hex> winningPath;
        public Player WinningPlayer;

        public Referee(int size = 11)
        {
            NewGame(size);
            AddPlayer(new Config(), 1);
            AddPlayer(new Config(), 2);
        }

        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public event EventHandler PlayerMadeMove;
        public event EventHandler GameOver;

        protected virtual void OnPlayerMadeMove(PlayerMadeMoveArgs e)
        {
            PlayerMadeMove?.Invoke(this, e);
        }

        public Player LosingPlayer()
        {
            if (WinningPlayer != null && WinningPlayer == Player1) return Player2;

            if (WinningPlayer != null) return Player1;

            return null;
        }

        protected virtual void OnGameEnd(GameOverArgs e)
        {
            Player1.GameOver(WinningPlayer.PlayerNumber);
            Player2.GameOver(WinningPlayer.PlayerNumber);
            GameOver?.Invoke(this, e);
        }

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
            if (CurrentPlayer() is HumanPlayer.HumanPlayer)
            {
                var player = (HumanPlayer.HumanPlayer) CurrentPlayer();
                player.ClickMadeOn(new Tuple<int, int>(x, y));
            }
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
                        Player1 = new HumanPlayer.HumanPlayer(playerNumber, Size, playerConfig);
                    else
                        Player2 = new HumanPlayer.HumanPlayer(playerNumber, Size, playerConfig);

                    break;
                case "Dozer AI":
                    if (playerNumber == 1)
                        Player1 = new Dozer.Dozer(playerNumber, Size, playerConfig);
                    else
                        Player2 = new Dozer.Dozer(playerNumber, Size, playerConfig);

                    break;
                case "Minimax Matrix AI":
                    if (playerNumber == 1)
                        Player1 = new MatrixPlayer(playerNumber, Size, playerConfig);
                    else
                        Player2 = new MatrixPlayer(playerNumber, Size, playerConfig);

                    break;
                case "Minimax List AI":
                    if (playerNumber == 1)
                        Player1 = new ListPlayer(playerNumber, Size, playerConfig);
                    else
                        Player2 = new ListPlayer(playerNumber, Size, playerConfig);

                    break;

                case "Replay AI":
                    if (playerNumber == 1)
                        Player1 = new Playback(playerNumber, Size, playerConfig);
                    else
                        Player2 = new Playback(playerNumber, Size, playerConfig);

                    break;

                default:
                    if (playerNumber == 1)
                        Player1 = new RandomPlayer.RandomPlayer(playerNumber, Size, null);
                    else
                        Player2 = new RandomPlayer.RandomPlayer(playerNumber, Size, null);
                    break;
            }
        }


        public async Task<Tuple<int, int>> TakeTurn(Player player)
        {
//            Quip(player.Name + " " + player.PlayerType() + " take your turn!");
            hexWanted = null;


            // Next, check to see if the player is the same as the last one
            if (player == _lastPlayer)
            {
                Quip("FOUL!  Taking a second turn!");
                GameEndsOnFoul();
            }

            if (_lastPlay == null) Quip("Last play is null.");
            hexWanted = await Task.Run(() => player.SelectHex(_lastPlay));

            if (hexWanted == null)
            {
                Quip("FOUL!  No hex was selected.  Player LOSES.");
                GameEndsOnFoul();
            }
            else
            {
                var success = Board.TakeHex(new Hex(hexWanted.Item1, hexWanted.Item2), CurrentPlayer().PlayerNumber);

                if (!success)
                {
                    Quip("FOUL!  Player tried to take a hex that was blocked!");
                    GameEndsOnFoul();
                }
                else
                {
                    if (CurrentPlayer().PlayerNumber == 1)
                        lastHexForPlayer1 = hexWanted;
                    else
                        lastHexForPlayer2 = hexWanted;

                    _lastPlay = hexWanted;
                    var playerMove = new Move
                    {
                        player = CurrentPlayer(),
                        hex = hexWanted
                    };
                    AllGameMoves.Add(playerMove);
                    var moveArgs = new PlayerMadeMoveArgs
                    {
                        player = CurrentPlayer().PlayerNumber,
                        move = new Tuple<int, int>(hexWanted.Item1, hexWanted.Item2)
                    };
                    //OnPlayerMadeMove(moveArgs);

                    LookForWinner();
                }
            }

            return new Tuple<int, int>(hexWanted.Item1, hexWanted.Item2);
        }

        private void GameEndsOnFoul()
        {
            WinningPlayer = OpponentPlayer();
            var args = new GameOverArgs
            {
                WinningPlayerNumber = WinningPlayer.PlayerNumber,
                WinningPath = null
            };
            OnGameEnd(args);
        }

        public void Dispose()
        {
            Size = 0;
            Board = null;
            Player1.GameOver(WinningPlayer.PlayerNumber);
            Player2.GameOver(WinningPlayer.PlayerNumber);
            Player1 = null;
            Player2 = null;
            winningPath = null;
            WinningPlayer = null;
            _lastPlayer = null;
        }

        private void PrintPath(List<Hex> path)
        {
            foreach (var hex in path) Console.Write("[" + hex.Row + "," + hex.Column + "] ");
            Console.WriteLine("");
        }

        public bool LookForWinner()
        {
            if (WinningPlayer != null) return true;

            var isHorizontal = CurrentPlayer().PlayerNumber != 1;

            if (CheckForWinningPath(CurrentPlayer().PlayerNumber))
            {
                WinningPlayer = CurrentPlayer();
                Quip("The winner is player #" + WinningPlayer.PlayerNumber + ", " + WinningPlayer.PlayerType() + "!");
                var args = new GameOverArgs
                {
                    WinningPlayerNumber = WinningPlayer.PlayerNumber,
                    WinningPath = Board.LastPathChecked
                };
                OnGameEnd(args);
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