using System.Drawing;
using Engine.Interfaces;

namespace Engine.GameTypes
{

    public class Hex
    {
        public int X;
        public int Y;
        public IPlayer Owner;
        public PointF Point;

        public Hex(int x, int y)
        {
            X = x;
            Point.X = x;
            Y = y;
            Point.Y = y;
        }
    }
}