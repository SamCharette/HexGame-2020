using System;
using System.Collections.Generic;
using System.Text;
using Engine.Interfaces;

namespace Engine.Players
{
    public class RandomPlayer : IPlayer
    {
        public string Name { get; set; }
        public int PlayerNumber { get; set; }
    }
}
