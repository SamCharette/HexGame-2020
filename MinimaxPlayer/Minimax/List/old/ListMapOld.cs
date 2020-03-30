//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using Players.Common;

//namespace Players.Minimax.List.old
//{
//    [Serializable]
//    public class ListMapOld
//    {
        

//        public object LockObject = new object();

//        public ListMap(int size)
//        {
//            Reset(size);
//        }

//        public ListMap()
//        {
//        }

//        private void DebugMessage(int level, string message)
//        {
//            if (Debug)
//            {
//                Console.WriteLine(new string('-', level) + message);
//            }
//        }


//        public bool Debug = true;
//        public string Name { get; set; }
//        public int Size { get; set; }
//        public List<ListHex> Board { get; set; }
//        public ListHex Top { get; set; }
//        public ListHex Bottom { get; set; }
//        public ListHex Left { get; set; }
//        public ListHex Right { get; set; }
//        public const string TopName = "Top";
//        public const string BottomName = "Bottom";
//        public const string LeftName = "Left";
//        public const string RightName = "Right";


//        public ListHex FindHex(int row, int col)
//        {
//            return FindHex(new Tuple<int, int>(row, col));
//        }

//        public ListHex FindHex(Tuple<int, int> coordinates)
//        {
//            if (coordinates.Item1 == -1) return Top;
//            if (coordinates.Item1 == -2) return Bottom;
//            if (coordinates.Item1 == Size * 2) return Left;
//            if (coordinates.Item1 == Size * 3) return Right;

//            var hexOnBoard = Board.FirstOrDefault(x => x.Row == coordinates.Item1 && x.Column == coordinates.Item2);

//            return hexOnBoard;
//        }

//        public bool TakeHex(PlayerType player, ListHex node)
//        {
//            return TakeHex(player, node.Row, node.Column);
//        }

//        public bool TakeHex(PlayerType player, Tuple<int, int> coordinates)
//        {
//            return TakeHex(player, coordinates.Item1, coordinates.Item2);
//        }

//        public bool TakeHex(PlayerType player, int row, int column)
//        {
//            lock (LockObject)
//            {
//                var hexToTake =
//                    Board.FirstOrDefault(x => x.Row == row
//                                              && x.Column == column
//                                              && x.Owner == PlayerType.White);
//                if (hexToTake == null) return false;
//                hexToTake.Owner = player;
//                var neighbours = GetFriendlyPhysicalNeighbours(hexToTake).ToList();
//                foreach (var neighbour in neighbours)
//                {
//                    AttachAllFriendlyNeighbours(hexToTake, neighbour);
//                    AttachAllFriendlyNeighbours(neighbour, hexToTake);
//                }

//                return true;
//            }
//        }

//        public bool ReleaseHex(Tuple<int, int> coordinates)
//        {
//            return ReleaseHex(coordinates.Item1, coordinates.Item2);
//        }

//        public bool ReleaseHex(ListHex hex)
//        {
//            return ReleaseHex(hex.Row, hex.Column);
//        }

//        public bool ReleaseHex(int row, int column)
//        {
//            lock (LockObject)
//            {
//                var hexToRelease = Board.FirstOrDefault(x => x.Row == row && x.Column == column);
//                if (hexToRelease != null)
//                {
//                    hexToRelease.Owner = PlayerType.White;
//                    foreach (var hex in Board) DetachHexes(hexToRelease, hex);
//                    DetachHexes(hexToRelease, Top);
//                    DetachHexes(hexToRelease, Bottom);
//                    DetachHexes(hexToRelease, Left);
//                    DetachHexes(hexToRelease, Right);

//                    return true;
//                }

//                return false;
//            }
//        }

//        public void Reset(int size)
//        {
//            Size = size;
//            Board = new List<ListHex>();
//            for (var row = 0; row < Size; row++)
//            for (var column = 0; column < Size; column++)
//            {
//                var hex = new ListHex(Size, row, column);
//                Board.Add(hex);
//            }

//            //Top = new ListHex(Size, -1, -1);
//            //Top.HexName = TopName;
//            //Top.Owner = PlayerType.Blue;
//            //Bottom = new ListHex(Size, Size * 2, Size * 2);
//            //Bottom.HexName = BottomName;
//            //Bottom.Owner = PlayerType.Blue;
//            //Left = new ListHex(Size, -2, -2);
//            //Left.HexName = LeftName;
//            //Left.Owner = PlayerType.Red;
//            //Right = new ListHex(Size, Size * 3, Size * 3);
//            //Right.HexName = RightName;
//            //Right.Owner = PlayerType.Red;
//        }

//        public void CleanPathingVariables()
//        {
//            foreach (var hex in Board) hex.ClearPathingVariables();
//        }

//        public List<ListHex> GetOpenPhysicalNeighbours(ListHex a)
//        {
//            return GetPhysicalNeighbours(a).Where(x => x.Owner == PlayerType.White).ToList();
//        }

//        public List<ListHex> GetTraversablePhysicalNeighbours(ListHex a, PlayerType player)
//        {
//            var opponent = player == PlayerType.Blue ? PlayerType.Red : PlayerType.Blue;
//            return GetPhysicalNeighbours(a).Where(x => x.Owner != opponent).ToList();
//        }

//        public List<ListHex> GetFriendlyPhysicalNeighbours(ListHex a)
//        {
//            return GetPhysicalNeighbours(a).Where(x => x.Owner == a.Owner).ToList();
//        }

//        public List<ListHex> GetPhysicalNeighbours(ListHex a)
//        {
//            if (a == Top) return Board.Where(x => x.Row == 0).ToList();

//            if (a == Bottom) return Board.Where(x => x.Row == Size - 1).ToList();

//            if (a == Left) return Board.Where(x => x.Column == 0).ToList();

//            if (a == Right) return Board.Where(x => x.Column == Size - 1).ToList();
//            var physicalNeighbours = new List<ListHex>();
//            foreach (var neighbourCoordinates in a.Neighbours)
//            {
//                var possibleNeighbour = FindHex(neighbourCoordinates.ToTuple());
//                if (possibleNeighbour != null) physicalNeighbours.Add(possibleNeighbour);
//            }

//            if (a.AttachedToTop) physicalNeighbours.Add(Top);
//            if (a.AttachedToBottom) physicalNeighbours.Add(Bottom);
//            if (a.AttachedToLeft) physicalNeighbours.Add(Left);
//            if (a.AttachedToRight) physicalNeighbours.Add(Right);

//            return physicalNeighbours;
//        }

     
//        public bool AreFriendlyNeighbours(ListHex a, ListHex b)
//        {
//            return a.Owner == b.Owner && ArePhysicalNeighbours(a, b);
//        }

//        public bool ArePhysicalNeighbours(ListHex a, ListHex b)
//        {
//            // First check to see if they are next to the ends
//            if (a.Equals(Top) && b.Row == 0 || a.Row == 0 && b.Equals(Top)) return true;
//            if (a.Equals(Left) && b.Column == 0 || a.Column == 0 && b.Equals(Left)) return true;
//            if (a.Equals(Bottom) && b.Row == Size - 1 || a.Row == Size - 1 && b.Equals(Bottom)) return true;
//            if (a.Equals(Right) && b.Column == Size - 1 || a.Column == Size - 1 && b.Equals(Right)) return true;

//            return a.Neighbours.Contains(new SimpleHex(b.Size, b.Row, b.Column));

//        }

//        public void AttachAllFriendlyNeighbours(ListHex a, ListHex b)
//        {
//            //AttachHexes(a, b);
//            //var toAttach = b.GetAttachedHexes().ToList();
//            //foreach (var listHex in toAttach) AttachHexes(a, listHex.Value);
//        }


//        public void AttachHexes(ListHex a, ListHex b)
//        {
//            if (a != null && b != null)
//            {
//                a.AttachTo(b);
//                b.AttachTo(a);
//            }
//        }

//        public void DetachHexes(ListHex a, ListHex b)
//        {
//            if (a != null && b != null)
//            {
//                a.DetachFrom(b);
//                b.DetachFrom(a);
//            }
//        }

//        //public bool CanHexReachBothEnds(ListHex hex, PlayerType me)
//        //{
//            //DebugMessage(0, "Checking " + hex + " for " + me);
//            //var start = me == PlayerType.Blue ? TopName : LeftName;
//            //var end = me == PlayerType.Blue ? BottomName : RightName;

//            //var touchesStart = hex.GetAttachedHexes().Any(x => x.Key == start);
//            //var touchesEnd = hex.GetAttachedHexes().Any(x => x.Key == end);
//            //if (touchesStart) DebugMessage(2,"Hex can reach out to start: " + hex.HexName);
//            //if (touchesEnd) DebugMessage(2, "Hex can reach out to the end: " + hex.HexName);

//            //var neighbours = 
//            //    GetPhysicalNeighbours(hex)
//            //        .Where(x => x.Owner == me)
//            //        .ToList();

//            //foreach (var neighbour in neighbours)
//            //{
//            //    DebugMessage(4,"Neighbour " + neighbour + " attached to " + neighbour.GetAttachedHexes().Count + " others.");
//            //    if (neighbour.GetAttachedHexes().Any(x => x.Key == start) || neighbour.HexName == start)
//            //    {
//            //        DebugMessage(8,neighbour + " can reach start.");
//            //        touchesStart = true;
//            //    }
//            //    if (neighbour.GetAttachedHexes().Any(x => x.Key == end) || neighbour.HexName == end)
//            //    {
//            //        DebugMessage(8, neighbour + " can reach end.");
//            //        touchesEnd = true;
//            //    }
//            //}
//            //return touchesStart && touchesEnd;
//        //}
//    }
//}

namespace MinimaxPlayer.Minimax.List.old
{
}