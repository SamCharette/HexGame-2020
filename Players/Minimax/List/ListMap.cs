using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text;
using Players.Common;
using Players.Minimax.List;

namespace Players.Minimax.List
{

    public class ListMap
    {
        public int Size;
        public List<ListNode> Board;
        public ListNode Top;
        public ListNode Bottom;
        public ListNode Left;
        public ListNode Right;

        public ListMap()
        {

        }



        public ListMap(int size)
        {
            Size = size;
            Board = new List<ListNode>(Size * Size);
            // Make the nodes
            Top = new ListNode(Size, -10, -10);
            Top.Location = NodeLocation.Top;
            Bottom = new ListNode(Size, 1000, 1000);
            Bottom.Location = NodeLocation.Bottom;
            Left = new ListNode(Size, -20, -20);
            Left.Location = NodeLocation.Left;
            Right = new ListNode(Size, 2000, 2000);
            Right.Location = NodeLocation.Right;

            for (var column = 0; column < Size; column++)
            {

                for (var row = 0; row < Size; row++)
                {
                    var newNode = new ListNode(Size, row, column);
                    if (column == 0)
                    {
                        AttachToEachOther(newNode, Left);
                    }

                    if (column == Size - 1)
                    {
                        AttachToEachOther(newNode, Right);
                    }

                    if (row == 0)
                    {
                        AttachToEachOther(newNode, Top);
                    }

                    if (row == Size - 1)
                    {
                        AttachToEachOther(newNode, Bottom);
                    }

                    Board.Add(newNode);
                }

                // Now go through the main nodes and attach all neighbours
                foreach (var node in Board)
                {
                    // Get and set adjacencies with neighbours
                    for (var i = 0; i < 6; i++)
                    {
                        var delta = Directions[(AxialDirections)i];
                        var neighbourNode =
                            Board.FirstOrDefault(x => x.Row == node.Row + delta.Item1
                                                      && x.Column == node.Column + delta.Item2);

                        if (neighbourNode != null)
                        {
                            AttachToEachOther(node, neighbourNode);
                        }

                    }
                }
                ClearPathValues();
            }
            Top.PingNeighbours();
            Bottom.PingNeighbours();
            Left.PingNeighbours();
            Right.PingNeighbours();
        }
        public void AttachToEachOther(ListNode a, ListNode b)
        {
            a.AttachTo(b);
            b.AttachTo(a);
        }

        public void DetachFromEachOther(ListNode a, ListNode b)
        {
            a.DetachFrom(b);
            b.DetachFrom(a);
        }

        public ListMap(ListMap mapToClone)
        {
            Size = mapToClone.Size;
            Top = new ListNode(mapToClone.Top);
            Bottom = new ListNode(mapToClone.Bottom);
            Left = new ListNode(mapToClone.Left);
            Right = new ListNode(mapToClone.Right);

            Board = new List<ListNode>(Size * Size);
            foreach (var node in mapToClone.Board)
            {
                var newNode = new ListNode(node);
                Board.Add(newNode);
            }
            foreach (var node in mapToClone.Board)
            {
                var clonedNode = Board.FirstOrDefault(x => x.Row == node.Row 
                                                           && x.Column == node.Column);

                foreach (var adjacentNode in node.Neighbours)
                {
                    var adjacentClonedNode =
                        Board.FirstOrDefault(x => x.Row == adjacentNode.Row 
                                                  && x.Column == adjacentNode.Column);
                    if (clonedNode != null && adjacentClonedNode != null)
                    {
                        AttachToEachOther(clonedNode, adjacentClonedNode);
                    }
                    else
                    {
                        if (adjacentNode.Row == -10 && adjacentNode.Column == -10)
                        {
                            AttachToEachOther(clonedNode, Top);
                        }
                        if (adjacentNode.Row == -20 && adjacentNode.Column == -20)
                        {
                            AttachToEachOther(clonedNode, Left);
                        }
                        if (adjacentNode.Row == 1000 && adjacentNode.Column == 1000)
                        {
                            AttachToEachOther(clonedNode, Bottom);
                        }
                        if (adjacentNode.Row == 2000 && adjacentNode.Column == 2000)
                        {
                            AttachToEachOther(clonedNode, Right);
                        }
                    }
                }
            }
        }

        public void ClearPathValues()
        {
            foreach (var node in Board)
            {
               
                node.G = 0;
                node.H = 0;
                node.Status = Status.Untested;
                node.Parent = null;

            }

            //Top.Status = Status.Untested;
            //Top.G = 0;
            //Top.H = 0;
            //Top.Owner = PlayerType.Blue;
            //Bottom.Status = Status.Untested;
            //Bottom.G = 0;
            //Bottom.H = 0;
            //Bottom.Owner = PlayerType.Blue;

            //Left.Status = Status.Untested;
            //Left.G = 0;
            //Left.H = 0;
            //Left.Owner = PlayerType.Red;
            //Right.Status = Status.Untested;
            //Right.G = 0;
            //Right.H = 0;
            //Right.Owner = PlayerType.Red;
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



        public bool TakeHex(Common.PlayerType owner, int row, int column)
        {
            var node = Board.FirstOrDefault(x =>
                x.Row == row && x.Column == column && x.Owner == Common.PlayerType.White);
            if (node == null)
            {
                return false;
            }

            node.Owner = owner;
            var neighboursToUpdate = FriendlyNeighboursOf(node);
            foreach (var neighbour in neighboursToUpdate)
            {
                AttachNeighboursFrom(node, neighbour);
            }
            node.PingNeighbours();

            return true;
        }

        public void AttachNeighboursFrom(ListNode source, ListNode current)
        {
            var friendlyNeighbours = FriendlyNeighboursOf(current);
            var notAddedYet =
                friendlyNeighbours.Where(x =>
                    !source.Neighbours.Any(y => y.Row == x.Row && y.Column == x.Column));
            foreach (var node in notAddedYet)
            {
                AttachToEachOther(source, node);
                AttachNeighboursFrom(source, node);
            }
        }

        public void ReleaseHex(int row, int column)
        {
            var node = Board.FirstOrDefault(x =>
                x.Row == row && x.Column == column);
            if (node != null)
            {
                node.Owner = PlayerType.White;
                node.PingNeighbours(false);
                foreach (var hex in Board)
                {
                    if (hex != Top && hex != Bottom && hex != Right && hex != Left)
                    {
                        DetachFromEachOther(hex, node);
                    }
                }
            }
        }

        public List<ListNode> FriendlyNeighboursOf(ListNode node)
        {
            if (node != null)
            {
                return PhysicalNeighboursOf(node).Where(x => x.Owner == node.Owner).ToList();
            }
            return new List<ListNode>();
        }
        public List<ListNode> PhysicalNeighboursOf(ListNode node)
        {
            var physicalNeighbours = new List<ListNode>();
            for (int i = 0; i < 6; i++)
            {
                var delta = Directions[(AxialDirections) i];
                var possibleNeighbour = Board.FirstOrDefault(x =>
                    x.Row == node.Row + delta.Item1 && x.Column == node.Column + delta.Item2);
                if (possibleNeighbour != null && possibleNeighbour.Location == NodeLocation.Board)
                {
                    physicalNeighbours.Add(possibleNeighbour);
                }
            }

            // Now add the exterior neighbours if they are there
            //if (node.IsNeighboursWith(Top))
            //{
            //    physicalNeighbours.Add(Top);
            //}
            //if (node.IsNeighboursWith(Bottom))
            //{
            //    physicalNeighbours.Add(Bottom);
            //}
            //if (node.IsNeighboursWith(Left))
            //{
            //    physicalNeighbours.Add(Left);
            //}
            //if (node.IsNeighboursWith(Right))
            //{
            //    physicalNeighbours.Add(Right);
            //}
            return physicalNeighbours;
        }
    }
}