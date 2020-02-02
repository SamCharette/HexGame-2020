using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.GameTypes;
using Engine.Interfaces;
using System.Threading.Tasks;

namespace Engine.Players
{
    public class RandomPlayer : IPlayer
    {
        public string Name { get; set; }
        public int PlayerNumber { get; set; }

        public RandomPlayer(int playerNumber)
        {
            PlayerNumber = playerNumber;
        }
        public Hex SelectHex(Board board)
        {
            var openHexes = board.Spaces.Where(x => x.Owner == null);
//            System.Threading.Thread.Sleep(2000);
            var selectedHex = openHexes.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
            return selectedHex;
        }
    }
}
