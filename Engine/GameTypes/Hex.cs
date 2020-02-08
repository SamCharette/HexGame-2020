using System.Drawing;
using Engine.Interfaces;

namespace Engine.GameTypes
{

    public class Hex
    {
        public IPlayer Owner;
        public PointF Point;

        public int X => (int) Point.X;

        public int Y => (int) Point.Y;
        // The cost in path is what a system can use to evaluate if a hex is desirable.
        public int CostInPath = 0;
        public Hex(int x, int y)
        {
            Point.X = x;
            Point.Y = y;
        }
    }
}