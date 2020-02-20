using System;


namespace HexLibrary
{
    public class Map
    {
        public Hex[,] Grid;
        public int Size;

        public Map(int size)
        {
            Size = size;
            for (var row = 0; row < size; row++)
            {
                for (var column = 0; column < size; column++)
                {
                    Grid[row, column] = new Hex(row, column);
                }
            }
        }

        public int OpponentNumber(int playerNumber)
        {
            if (playerNumber == 1)
            {
                return 2;
            }

            return 1;
        }

        public void TakeHex(Hex a, int playerNumber)
        {
            if (!IsBlockedByOpponent(a, OpponentNumber(playerNumber)))
            {
                a.OwnerNumber = playerNumber;
            }
        }

        public bool AreConnected(Hex a, Hex b)
        {
            throw new NotImplementedException();
        }

        public int DistanceBetween(Hex a, Hex b)
        {
            return a.DistanceTo(b);
        }

        public bool DoesPathExistBetween(Hex a, Hex b)
        {
            throw new NotImplementedException();
        }

        public bool IsAtTop(Hex a)
        {
            return a.r == 0;
        }

        public bool IsAtBottom(Hex a)
        {
            return a.r == Size - 1;
        }

        public bool IsAtLeft(Hex a)
        {
            return a.q == 0;
        }

        public bool IsAtRight(Hex a)
        {
            return a.q == Size - 1;
        }

        public bool IsBlockedByEdge(Hex a)
        {
            return a.r < 0 || a.r >= Size || a.q < 0 || a.q >= Size;
        }

        public bool IsBlockedByOpponent(Hex a, int opponentNumber)
        {
            return a.OwnerNumber == opponentNumber;
        }

        public bool IsBlocked(Hex a, int opponentNumber = 0)
        {
            var isBlockedByEdge = IsBlockedByEdge(a);
            if (!isBlockedByEdge && opponentNumber != 0)
            {
                return IsBlockedByOpponent(a, opponentNumber);
            }

            return isBlockedByEdge;
        }
    }
}
