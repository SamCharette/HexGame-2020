using System;
using System.Collections.Generic;
using System.Text;
using HexLibrary;

namespace Engine
{
    public class GameOverArgs : EventArgs
    {
        public int WinningPlayerNumber;
        public List<Hex> WinningPath;
    }
}
