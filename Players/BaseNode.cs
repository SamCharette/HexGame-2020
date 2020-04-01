using System;

namespace Players
{
    public class BaseNode
    {
        public int Row;
        public int Column;
        public Status Status;
        public int G;
        public int H;
        public BaseNode Parent = null;
        public Guid RandomValue;
        public int Owner;

        public BaseNode()
        {
            Row = 0;
            Column = 0;
            Status = Status.Untested;
            G = 0;
            H = 0;
            Parent = null;
            Owner = 0;
            RandomValue = Guid.NewGuid();
        }

        public int F => G + H;

        public int EnemyPlayerNumber()
        {
            return Owner == 1 ? 2 : 1;
        }

        public void ClearPathingVariables()
        {
            G = 0;
            H = 0;
            Owner = 0;
            Status = Status.Untested;
            Parent = null;
        }

        public Tuple<int,int> ToTuple()
        {
            return new Tuple<int, int>(Row, Column);
        }

        public bool Equal(BaseNode node)
        {
            return Row == node.Row && Column == node.Column;
        }

        public bool CanWalkTo(BaseNode possibleNeighbour)
        {
            // Can't be a neighbour to itself
            if (Row == possibleNeighbour.Row && Column == possibleNeighbour.Column)
            {
                return false;
            }

            if (possibleNeighbour.Status == Status.Closed)
            {
                return false;
            }
            // Can't walk if enemy owned
            if (possibleNeighbour.Owner == EnemyPlayerNumber())
            {
                return false;
            }
            // Top right
            if (Row == possibleNeighbour.Row + 1 && Column == possibleNeighbour.Column - 1)
            {
                return true;
            }
            // Right
            if (Row == possibleNeighbour.Row + 1 && Column == possibleNeighbour.Column)
            {
                return true;
            }
            // Bottom right
            if (Row == possibleNeighbour.Row && Column == possibleNeighbour.Column + 1)
            {
                return true;
            }
            // Bottom left
            if (Row == possibleNeighbour.Row - 1 && Column == possibleNeighbour.Column + 1)
            {
                return true;
            }
            // Left
            if (Row == possibleNeighbour.Row - 1 && Column == possibleNeighbour.Column)
            {
                return true;
            }
            // Top Left
            if (Row == possibleNeighbour.Row && Column == possibleNeighbour.Column - 1)
            {
                return true;
            }
            return false;
        }
    }
}