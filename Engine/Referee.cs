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

        // we need a player 1
        private IPlayer _player1;

        // we need a player 2
        private IPlayer _player2;

        private IPlayer CurrentPlayer()
        {
            return _player1 == _lastPlayer ? _player2 : _player1;
        }

        private void SwitchPlayers()
        {
            _lastPlayer = _player1 == _lastPlayer ? _player2 : _player1;
        }


        // We need a way to get input from a player

        // We need a way to send the new board and game state to a player


        // functions
        public Referee(int size = 11)
        {
            Size = size;
            NewBoard();
            _player1 = new RandomPlayer(1);
            _player2 = new RandomPlayer(2);
        }

        public void NewGame(int size = 11)
        {
            Size = size;
            NewBoard();
            Play();
        }

        public Board Play()
        {
            try
            {
                while (Board.Winner(_lastPlayer) == null)
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
            if (Board.TakeHex(hexWanted.X, hexWanted.Y, CurrentPlayer()))
            {
                SwitchPlayers();
            };
            return hexWanted;

        }


        public void NewBoard()
        {
            Board = new Board(Size);

        }
    }
}