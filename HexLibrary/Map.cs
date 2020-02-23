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
        public List<Hex> LastPathChecked = null;

        public Map(int size)
        {
            Size = size;
            Grid = new Hex[Size, Size];
            for (var column = 0; column < size; column++)
            {
                for (var row = 0; row < size; row++)
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

        public bool TakeHex(Hex a, int playerNumber)
        {
            if (!IsBlocked(a, OpponentNumber(playerNumber)))
            {
                Grid[a.Row, a.Column].OwnerNumber = playerNumber;
                return true;
            }

            return false;
        }

        public bool AreNeighbours(Hex a, Hex b)
        {
            for (int direction = 0; direction < 6; direction++)
            {
                var neighbour = NeighbourAt(a, (AxialDirections)direction);
                if (neighbour.Row == b.Row && neighbour.Column == b.Column)
                {
                    return true;
                }
            }

            return false;
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
            if (playerNumber == 2)
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
            fringes[0] = new List<Hex>();
            fringes[0].Add(a);
            for (int step = 1; step < (Size * Size) && fringes[step - 1].Any(); step++)
            {
                fringes[step] = new List<Hex>();
                foreach (var fringe in fringes[step - 1])
                {
                    for (var direction = 0; direction < 6; direction++)
                    {
                        var hex = NeighbourAt(fringe, (AxialDirections) direction);
                        if (AlreadyBelongsTo(hex, a.OwnerNumber) && !visited.Any(x => x.q == hex.q && x.r == hex.r))
                        {
                            visited.Add(hex);
                            if (hex.q == b.q && hex.r == b.r)
                            {
                                LastPathChecked = visited;
                                return true;
                            }
                            fringes[step].Add(hex);
                        }
                    }
                }
            }

            return false;

        }

        public Dictionary<AxialDirections, Tuple<int, int>> Directions = new Dictionary<AxialDirections, Tuple<int, int>>()
        {
            { AxialDirections.TopLeft, new Tuple<int, int>(0, -1) },
            { AxialDirections.TopRight, new Tuple<int, int>(+1, -1) },
            { AxialDirections.Right, new Tuple<int, int>(+1, 0) },
            { AxialDirections.BottomRight, new Tuple<int, int>(0, +1) },
            { AxialDirections.BottomLeft, new Tuple<int, int>(-1, +1) },
            { AxialDirections.Left, new Tuple<int, int>(-1, 0) }
        };

        public Dictionary<AxialDiagonalDirections, Tuple<int, int>> DiagonalDirections = new Dictionary<AxialDiagonalDirections, Tuple<int, int>>()
        {
            { AxialDiagonalDirections.Top,  new Tuple<int, int>(+1, -2) },
            { AxialDiagonalDirections.TopRight, new Tuple<int, int>(+2, -1) },
            { AxialDiagonalDirections.BottomRight, new Tuple<int, int>(+1, +1) },
            { AxialDiagonalDirections.Bottom, new Tuple<int, int>(-1, +2) },
            { AxialDiagonalDirections.BottomLeft, new Tuple<int, int>(-2, +1) },
            { AxialDiagonalDirections.TopLeft, new Tuple<int, int>(-1, -1) }
        };

        public Hex NeighbourAt(Hex a, AxialDirections direction)
        {
            var delta = Directions[direction];
            var row = a.Row + delta.Item1;
            var column = a.Column + delta.Item2;

            if (row >= 0 && row < Size && column >= 0 && column < Size)
            {
                return Grid[row, column];
            }

            return null;
        }

        public bool AlreadyBelongsTo(Hex a, int playerNumber)
        {
            return !IsBlocked(a, OpponentNumber(playerNumber)) && a.OwnerNumber == playerNumber;
        }

        public bool CanBeStartHexForPlayer(Hex hex, int playerNumber)
        {
            if (AlreadyBelongsTo(hex, playerNumber))
            {
                return playerNumber == 1 ? IsAtTop(hex) : IsAtLeft(hex);
            }
            

            return false;
        }

        public bool CanBeEndHexForPlayer(Hex hex, int playerNumber)
        {
            if (AlreadyBelongsTo(hex, playerNumber))
            {
                return playerNumber == 1 ? IsAtBottom(hex) : IsAtRight(hex);
            }


            return false;
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
            if (a == null)
            {
                return true;
            }
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
