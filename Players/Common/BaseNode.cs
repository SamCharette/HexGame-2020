using System;
using Players.Common;

namespace Players.Base
{
    public class BaseNode
    {
        public int X;
        public int Y;
        public Status Status;
        public int G;
        public int H;
        public BaseNode Parent = null;
        public Guid uniqueness;
        public int Owner;

        public int F => G + H;

        public bool CanWalkTo(BaseNode possibleNeighbour)
        {
            // Can't be a neighbour to itself
            if (X == possibleNeighbour.X && Y == possibleNeighbour.Y)
            {
                return false;
            }

            if (possibleNeighbour.Status == Status.Closed)
            {
                return false;
            }
            // Can't walk if enemy owned
            if (possibleNeighbour.Owner != Owner && possibleNeighbour.Owner != 0)
            {
                return false;
            }
            // Top right
            if (X == possibleNeighbour.X + 1 && Y == possibleNeighbour.Y - 1)
            {
                return true;
            }
            // Right
            if (X == possibleNeighbour.X + 1 && Y == possibleNeighbour.Y)
            {
                return true;
            }
            // Bottom right
            if (X == possibleNeighbour.X && Y == possibleNeighbour.Y + 1)
            {
                return true;
            }
            // Bottom left
            if (X == possibleNeighbour.X - 1 && Y == possibleNeighbour.Y + 1)
            {
                return true;
            }
            // Left
            if (X == possibleNeighbour.X - 1 && Y == possibleNeighbour.Y)
            {
                return true;
            }
            // Top Left
            if (X == possibleNeighbour.X && Y == possibleNeighbour.Y - 1)
            {
                return true;
            }
            return false;
        }
    }
}