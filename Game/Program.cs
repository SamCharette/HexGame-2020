using System;
using Engine;

namespace Game
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Game Start!");
            var referee = new Referee();
            referee.NewGame();
            referee.Play();
        }
    }
}
