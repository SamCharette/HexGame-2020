using System.Drawing;
using Engine.Interfaces;

namespace Engine.GameTypes
{

    public class Hex
    {
        public int Owner = 0;
        public PointF Point;

        public int X => (int) Point.X;

        public int Y => (int) Point.Y;
        // The cost in path is what a system can use to evaluate if a hex is desirable.
        public Hex(int x, int y)
        {
            Point = new PointF(x, y);
        }
    }
}