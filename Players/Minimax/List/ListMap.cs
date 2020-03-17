using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Players.Common;

namespace Players.Minimax.List
{
    [Serializable]
    public class ListMap
    {
        public int Size { get; set; }
        public List<ListHex> Board { get; set; }
        public ListHex Top { get; set; }
        public ListHex Bottom { get; set; }
        public ListHex Left { get; set; }
        public ListHex Right { get; set; }

        public ListMap(int size)
        {
            Reset(size);
        }
        public ListMap()
        {

        }

        public  ListMap(ListMap source)
        {
            var newMap = new ListMap(source.Size);
            newMap.Reset(Size);
            foreach (var hex in source.Board.ToList())
            {
                var newHex = newMap.Board.FirstOrDefault(x => x.Row == hex.Row && x.Column == hex.Column);
                if (newHex != null)
                {
                    foreach (var neighbour in hex.Attached)
                    {
                        ListHex newNeighbour;
                        if (neighbour.HexName == "Top")
                        {
                            newNeighbour = newMap.Top;
                        }
                        else if (neighbour.HexName == "Bottom")
                        {
                            newNeighbour = newMap.Bottom;
                        }
                        if (neighbour.HexName == "Left")
                        {
                            newNeighbour = newMap.Left;
                        }
                        if (neighbour.HexName == "Right")
                        {
                            newNeighbour = newMap.Right;
                        }
                        else
                        {
                            newNeighbour = newMap.Board.FirstOrDefault(x => x.Row == neighbour.Row && x.Column == neighbour.Column);
                        }

                        if (newNeighbour != null)
                        {
                            newHex.Attached.Add(newNeighbour);
                        }

                    }

                }
            }
            
        }
        public ListHex FindHex(int row, int col)
        {
            return FindHex(new Tuple<int, int>(row, col));
        }
        public ListHex FindHex(Tuple<int, int> coordinates)
        {
            if (coordinates.Item1 == -1)
            {
                return Top;
            }
            if (coordinates.Item1 == -2)
            {
                return Bottom;
            }
            if (coordinates.Item1 == Size * 2)
            {
                return Left;
            }
            if (coordinates.Item1 == Size * 3)
            {
                return Right;
            }

            var hexOnBoard = Board.FirstOrDefault(x => x.Row == coordinates.Item1 && x.Column == coordinates.Item2);

            return hexOnBoard;

        }

        public bool TakeHex(PlayerType player, ListHex node)
        {
            return TakeHex(player, node.Row, node.Column);
        }

        public bool TakeHex(PlayerType player, Tuple<int,int> coordinates)
        {
            return TakeHex(player, coordinates.Item1, coordinates.Item2);
        }

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
            var neighbours = GetFriendlyPhysicalNeighbours(hexToTake).ToList();
            foreach (var neighbour in neighbours)
            {
                AttachAllFriendlyNeighbours(hexToTake, neighbour);
                AttachAllFriendlyNeighbours(neighbour, hexToTake);
            }

            return true;
        }

        public bool ReleaseHex(Tuple<int, int> coordinates)
        {
            return ReleaseHex(coordinates.Item1, coordinates.Item2);
        }
        public bool ReleaseHex(ListHex hex)
        {
            return ReleaseHex(hex.Row, hex.Column);
        }
        public bool ReleaseHex(int row, int column)
        {
            var hexToRelease = Board.FirstOrDefault(x => x.Row == row && x.Column == column);
            if (hexToRelease != null)
            {
                hexToRelease.Owner = PlayerType.White;
                Board.ForEach(x => DetachHexes(x, hexToRelease));
                DetachHexes(hexToRelease, Top);
                DetachHexes(hexToRelease, Bottom);
                DetachHexes(hexToRelease, Left);
                DetachHexes(hexToRelease, Right);

                return true;
            }
            return false;
        }

        public void Reset(int size)
        {
            Size = size;
            Board = new List<ListHex> ();
            for (var row = 0; row < Size; row++)
            {
                for (var column = 0; column < Size; column++)
                {
                    var hex = new ListHex(Size, row, column);
                    Board.Add(hex);
                }
            }
            Top = new ListHex(Size, -1, -1);
            Top.HexName = "Top";
            Top.Owner = PlayerType.Blue;
            Bottom = new ListHex(Size, Size * 2, Size * 2 );
            Bottom.HexName = "Bottom";
            Bottom.Owner = PlayerType.Blue;
            Left = new ListHex(Size, -2, -2);
            Left.HexName = "Left";
            Left.Owner = PlayerType.Red;
            Right = new ListHex(Size, Size * 3, Size * 3);
            Right.HexName = "Right";
            Right.Owner = PlayerType.Red;

            
        }
        public void CleanPathingVariables()
        {
            foreach (var hex in Board)
            {
                hex.ClearPathingVariables();
            }
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

        public List<ListHex> GetOpenPhysicalNeighbours(ListHex a)
        {
            return GetPhysicalNeighbours(a).Where(x => x.Owner == PlayerType.White).ToList();

        }
        public List<ListHex> GetTraversablePhysicalNeighbours(ListHex a, Common.PlayerType player)
        {
            var opponent = player == PlayerType.Blue ? PlayerType.Red : PlayerType.Blue;
            return GetPhysicalNeighbours(a).Where(x => x.Owner != opponent).ToList();

        }
        public List<ListHex> GetFriendlyPhysicalNeighbours(ListHex a)
        {
            return GetPhysicalNeighbours(a).Where(x => x.Owner == a.Owner).ToList();
        }
        public List<ListHex> GetPhysicalNeighbours(ListHex a)
        {
            if (a == Top)
            {
                return Board.Where(x => x.Row == 0).ToList();
            }

            if (a == Bottom)
            {
                return Board.Where(x => x.Row == Size - 1).ToList();
            }

            if (a == Left)
            {
                return Board.Where(x => x.Column == 0).ToList();
            }

            if (a == Right)
            {
                return Board.Where(x => x.Column == Size - 1).ToList();
            }
            var physicalNeighbours = new List<ListHex>();
            for (var i = 0; i < 6; i++)
            {
                var delta = Directions[(AxialDirections) i];
                var possibleNeighbour = FindHex(a.AddDelta(delta));
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
            if (a.Equals(Top) && b.Row == 0 || a.Row == 0 && b.Equals(Top))
            {
                return true;
            }
            if (a.Equals(Left) && b.Column == 0 || a.Column == 0 && b.Equals(Left))
            {
                return true;
            }
            if (a.Equals(Bottom) && b.Row == Size - 1 || a.Row == Size - 1 && b.Equals(Bottom))
            {
                return true;
            }
            if (a.Equals(Right) && b.Column == Size - 1 || a.Column == Size - 1 && b.Equals(Right))
            {
                return true;
            }

            // Otherwise, check the physical neighbours via direction
            for (var i = 0; i < 6; i++)
            {
                var delta = Directions[(AxialDirections) i];
                var newLocation = a.AddDelta(delta);
                var hex = FindHex(newLocation);
                if (hex != null && b.Equals(hex))
                {
                    return true;
                }
            }
            return false;
        }

        public void AttachAllFriendlyNeighbours(ListHex a, ListHex b)
        {
            AttachHexes(a, b);
            var friendsOfFriends = 
                b.Attached.Where(x => x.Owner == a.Owner).ToList();
            foreach (var node in friendsOfFriends.ToList())
            {
                AttachHexes(a, node);
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
