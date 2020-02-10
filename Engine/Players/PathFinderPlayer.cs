using System;
using System.Collections.Generic;
using System.Text;
using Engine.GameTypes;
using Engine.Interfaces;

namespace Engine.Players
{
    public class PathFinderPlayer : IPlayer
    {
        public string Name { get; set; }
        public int PlayerNumber { get; set; }
        public Hex SelectHex(Board board)
        {
            throw new NotImplementedException();
        }
    }
}
