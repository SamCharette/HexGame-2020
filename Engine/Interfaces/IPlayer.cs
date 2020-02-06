using System;
using System.Collections.Generic;
using System.Text;
using Engine.GameTypes;

namespace Engine.Interfaces
{
    public interface IPlayer
    {
        string Name { get; set; }
        int PlayerNumber { get; set; }
        Hex SelectHex(Board board);
    }
}
