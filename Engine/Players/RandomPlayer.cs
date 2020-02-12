using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.GameTypes;
using Engine.Interfaces;
using System.Threading.Tasks;

namespace Engine.Players
{
    public class RandomPlayer : Player
    {

        public new string PlayerType()
        {
            return "Random AI";
        }
        public new bool IsAvailableToPlay()
        {
            return true;
        }
        public RandomPlayer(int playerNumber, int boardSize) : base(playerNumber, boardSize)
        {
        }
    }
}
