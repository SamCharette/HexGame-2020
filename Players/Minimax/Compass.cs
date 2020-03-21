using System;
using System.Collections.Generic;
using System.Text;

namespace Players.Minimax
{
    public static class Compass
    {
        private static readonly Dictionary<AxialDirections, Tuple<int, int>> Directions =
            new Dictionary<AxialDirections, Tuple<int, int>>
            {
                {AxialDirections.TopLeft, new Tuple<int, int>(0, -1)},
                {AxialDirections.TopRight, new Tuple<int, int>(+1, -1)},
                {AxialDirections.Right, new Tuple<int, int>(+1, 0)},
                {AxialDirections.BottomRight, new Tuple<int, int>(0, +1)},
                {AxialDirections.BottomLeft, new Tuple<int, int>(-1, +1)},
                {AxialDirections.Left, new Tuple<int, int>(-1, 0)}
            };

        private static Tuple<int,int> GetCoordinatesFor(Tuple<int,int> coordinates, AxialDirections direction)
        {
            var newCoordinates = new Tuple<int,int>(coordinates.Item1 + Directions[direction].Item1,
                coordinates.Item2 + Directions[direction].Item2);
            return Directions[direction];
        }

        public static Tuple<int, int> GetCoordinatesFor(Tuple<int, int> coordinates, int direction)
        {
            return GetCoordinatesFor(coordinates, (AxialDirections) direction);
        }

        public static Tuple<int,int> GetDeltaFor(AxialDirections direction)
        {
            return Directions[direction];
        }

        public static Tuple<int,int> GetDeltaFor(int direction)
        {
            return GetDeltaFor((AxialDirections) direction);
        }
    }
}
