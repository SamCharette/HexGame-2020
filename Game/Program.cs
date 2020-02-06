using System;
using Engine;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Game Start!");
            var referee = new Referee();
            referee.NewGame();


                do
                {
                    if (referee.Board.Spaces.Count(x => x.Owner == null) == 0)
                    {
                        break;
                    }
                    Console.WriteLine("Player taking turn: " + referee.CurrentPlayer().PlayerNumber);

                    var hexTaken = referee.TakeTurn(referee.CurrentPlayer());
                    if (hexTaken != null)
                    {
                        Console.WriteLine("Hex selected was : " + hexTaken.X + ", " + hexTaken.Y);

                    }
                } while (!referee.Winner());

                if (referee.Board.Spaces.Count(x => x.Owner == null) == 0)
                {
                    Console.WriteLine("The game is a draw.");
                }
                else
                {
                    Console.WriteLine("The winner is Player #" + referee.CurrentPlayer().PlayerNumber);
                }
       
        }
    }
}
