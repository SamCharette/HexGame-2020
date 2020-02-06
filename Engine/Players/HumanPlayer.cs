using System;
using System.Threading;
using System.Threading.Tasks;
using Engine.GameTypes;
using Engine.Interfaces;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Engine.Players
{
    public class HumanPlayer : IPlayer
    {
        public string Name { get; set; }
        public int PlayerNumber { get; set; }
     
        public Hex SelectHex(Board board)
        {
            while (board.clickedHex == null)
            {

            }

            var selectedHex = board.clickedHex;
            board.clickedHex = null;
            return selectedHex;
        }
    }
}
