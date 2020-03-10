using System;
using System.Collections.Generic;
using System.Linq;
using Players.Common;

namespace Players.Minimax.List
{
    public class ListMap
    {
        public int Size { get; set; }
        public List<ListHex> Board { get; set; }
        public ListHex Top { get; set; }
        public ListHex Bottom { get; set; }
        public ListHex Left { get; set; }
        public ListHex Right { get; set; }


        public bool TakeHex(Common.PlayerType player, int row, int column)
        {
            var hexToTake =
                Board.FirstOrDefault(x => x.Row == row 
                                          && x.Column == column 
                                          && x.Owner == PlayerType.White);
            if (hexToTake == null)
            {
                return false;
            }
            hexToTake.Owner = player;
            var neighbours = GetFriendlyPhysicalNeighbours(hexToTake);
            foreach (var neighbour in neighbours)
            {
                AttachAllFriendlyNeighbours(hexToTake, neighbour);
            }

            return true;
        }

        public bool ReleaseHex(int row, int column)
        {
            var hexToRelease = Board.FirstOrDefault(x => x.Row == row && x.Column == column);
            if (hexToRelease != null)
            {
                Board.ForEach(x => x.DetachFrom(hexToRelease));
                return true;
            }
            return false;
        }

        public void Reset(int size)
        {
            Size = size;
            Board = new List<ListHex> (Size * Size);
            Top = new ListHex(Size, -1, -1);
            Top.Owner = PlayerType.Blue;
            Bottom = new ListHex(Size, Size * 2, Size * 2 );
            Bottom.Owner = PlayerType.Blue;
            Left = new ListHex(Size, -2, -2);
            Left.Owner = PlayerType.Red;
            Right = new ListHex(Size, Size * 2, Size * 3);
            Right.Owner = PlayerType.Red;

            
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

        public List<ListHex> GetFriendlyPhysicalNeighbours(ListHex a)
        {
            return GetPhysicalNeighbours(a).Where(x => x.Owner == a.Owner).ToList();
        }
        public List<ListHex> GetPhysicalNeighbours(ListHex a)
        {
            var physicalNeighbours = new List<ListHex>();
            for (var i = 0; i < Size; i++)
            {
                var delta = Directions[(AxialDirections) i];
                var possibleNeighbour =
                    Board.FirstOrDefault(x => x.Row == a.Row + delta.Item1 && x.Column == a.Column + delta.Item2);
                if (possibleNeighbour != null)
                {
                    physicalNeighbours.Add(possibleNeighbour);
                }
            }

            if (IsHexAtTop(a))
            {
                physicalNeighbours.Add(Top);
            }
            if (IsHexAtBottom(a))
            {
                physicalNeighbours.Add(Bottom);
            }
            if (IsHexAtLeft(a))
            {
                physicalNeighbours.Add(Left);
            }
            if (IsHexAtRight(a))
            {
                physicalNeighbours.Add(Right);
            }

            return physicalNeighbours;
        }

        public bool IsHexAtTop(ListHex a)
        {
            return a.Row == 0;
        }
        public bool IsHexAtBottom(ListHex a)
        {
            return a.Row == Size - 1;
        }
        public bool IsHexAtLeft(ListHex a)
        {
            return a.Column == 0;
        }
        public bool IsHexAtRight(ListHex a)
        {
            return a.Column == Size - 1;
        }
        public bool AreFriendlyNeighbours(ListHex a, ListHex b)
        {
            return a.Owner == b.Owner && ArePhysicalNeighbours(a, b);
        }

        public bool ArePhysicalNeighbours(ListHex a, ListHex b)
        {
            // First check to see if they are next to the ends
            if (a == Top && b.Row == 0 || a.Row == 0 && b == Top)
            {
                return true;
            }
            if (a == Left && b.Column == 0 || a.Column == 0 && b == Left)
            {
                return true;
            }
            if (a == Bottom && b.Row == Size - 1 || a.Row == Size - 1 && b == Bottom)
            {
                return true;
            }
            if (a == Right && b.Row == Size - 1 || a.Row == Size - 1 && b == Right)
            {
                return true;
            }

            // Otherwise, check the physical neighbours via direction
            for (var i = 0; i < 6; i++)
            {
                var delta = Directions[(AxialDirections) i];
                var newLocation = new Tuple<int, int>(a.Row + delta.Item1, a.Column + delta.Item2);
                if (newLocation.Item1 >= 0 && newLocation.Item1 < Size && newLocation.Item2 >= 0 && newLocation.Item2 < Size)
                {
                    return true;
                }
            }
            return false;
        }

        public void AttachAllFriendlyNeighbours(ListHex a, ListHex b)
        {
            AttachHexes(a, b);
            foreach (var node in b.Attached)
            {
                AttachAllFriendlyNeighbours(a, node);
            }
        }

        
        public void AttachHexes(ListHex a, ListHex b)
        {
            a.AttachTo(b);
            b.AttachTo(a);
        }

        public void DetachHexes(ListHex a, ListHex b)
        {
            a.DetachFrom(b);
            b.DetachFrom(a);
        }

    }
}
