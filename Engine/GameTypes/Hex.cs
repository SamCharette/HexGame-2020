using Engine.Interfaces;

namespace Engine.GameTypes
{

    public class Hex
    {
        public int X;
        public int Y;
        public IPlayer Owner;

        public Hex(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}