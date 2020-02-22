using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;


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
            if (!IsBlocked(a, OpponentNumber(playerNumber)))
            {
                a.OwnerNumber = playerNumber;
            }
        }

        public bool AreNeighbours(Hex a, Hex b)
        {
            return a.IsANeighbourOf(b);
        }

        public int DistanceBetween(Hex a, Hex b)
        {
            return a.DistanceTo(b);
        }

        public int DistanceToTop(Hex a)
        {
            return a.Row;
        }

        public int DistanceToBottom(Hex a)
        {
            return Size - 1 - a.Row;
        }

        public int DistanceToLeft(Hex a)
        {
            return a.Column;
        }

        public int DistanceToRight(Hex a)
        {
            return Size - 1 - a.Column;
        }

        public bool DoesWinningPathExistFor(int playerNumber)
        {
            var leftMost = new List<Hex>();
            var topMost = new List<Hex>();
            var rightMost = new List<Hex>();
            var bottomMost = new List<Hex>();

            foreach (var hex in Grid)
            {
                if (AlreadyBelongsTo(hex, playerNumber))
                {
                    if (IsAtLeft(hex))
                    {
                        leftMost.Add(hex);
                    }
                    if (IsAtTop(hex))
                    {
                        topMost.Add(hex);
                    }

                    if (IsAtRight(hex))
                    {
                        rightMost.Add(hex);
                    }

                    if (IsAtBottom(hex))
                    {
                        bottomMost.Add(hex);
                    }
                }
            }
            // TO DO should probably dry this up a bit
            if (playerNumber == 1)
            {
                foreach (var startingHex in leftMost)
                {
                    foreach (var endingHex in rightMost)
                    {
                        if (DoesPathExistBetween(startingHex, endingHex))
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                foreach (var startingHex in topMost)
                {
                    foreach (var endingHex in bottomMost)
                    {
                        if (DoesPathExistBetween(startingHex, endingHex))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public bool DoesPathExistBetween(Hex a, Hex b)
        {
   
            var visited = new List<Hex>();
            visited.Add(a);
            var fringes = new List<Hex>[Size * Size];
            fringes[0].Add(a);
            for (int step = 1; step < (Size * Size); step++)
            {
                foreach (var fringe in fringes[step - 1])
                {
                    for (var direction = 0; direction < 6; direction++)
                    {
                        var hex = fringe.Neighbour((AxialDirections) direction);
                        if (AlreadyBelongsTo(hex, a.OwnerNumber) && !visited.Any(x => x.q == a.q && x.r == a.r))
                        {
                            if (hex.q == b.q && hex.r == b.r)
                            {
                                return true;
                            }
                            visited.Add(hex);
                            fringes[step].Add(hex);
                        }
                    }
                }
            }

            return false;

        }

        public bool AlreadyBelongsTo(Hex a, int playerNumber)
        {
            return !IsBlocked(a, OpponentNumber(playerNumber)) && a.OwnerNumber == playerNumber;
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
