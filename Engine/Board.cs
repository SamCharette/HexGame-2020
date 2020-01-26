using System;
using System.Collections.Generic;
using System.Text;
using Engine.GameTypes;
using Engine.Interfaces;

namespace Engine
{
    public class Board
    {
        public List<Hex> Spaces;

        public Board()
        {
            for (var i = 0; i < 11; i++)
            {
                for (var j = 0; j < 11; j++)
                {
                    var hex = new Hex(i, j);
                    Spaces.Add(hex);
                }
            }
        }
        public bool TakeHex(int x, int y, IPlayer player)
        {

        }

        public bool CheckHex(int x, int y)
        {

        }
    }
}
