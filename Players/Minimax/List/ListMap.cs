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
            Bottom = new ListNode(Size, 1000, 1000);
            Left = new ListNode(Size, -20, -20);
            Right = new ListNode(Size, 2000, 2000);

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
        }
        

    


        public void AttachToEachOther(ListNode a, ListNode b)
        {
            a.AttachTo(b);
            b.AttachTo(a);
        }

        public ListMap(ListMap mapToClone)
        {
            Size = mapToClone.Size;
            Board = new List<ListNode>(Size * Size);
            foreach (var node in mapToClone.Board)
            {
                var newNode = new ListNode(Size, node.Row, node.Column);
                newNode.Owner = node.Owner;
                Board.Add(newNode);
            }
            foreach (var node in mapToClone.Board)
            {
                var clonedNode = Board.FirstOrDefault(x => x.Row == node.Row && x.Column == node.Column);
                foreach (var adjacentNode in node.Neighbours)
                {
                    var adjacentClonedNode =
                        Board.FirstOrDefault(x => x.Row == adjacentNode.Row && x.Column == adjacentNode.Column);
                    clonedNode.Neighbours.Add(adjacentClonedNode);
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

            }

            Top.Status = Status.Untested;
            Top.G = 0;
            Top.H = 0;
            Top.Owner = PlayerType.Blue;
            Bottom.Status = Status.Untested;
            Bottom.G = 0;
            Bottom.H = 0;
            Bottom.Owner = PlayerType.Blue;

            Left.Status = Status.Untested;
            Left.G = 0;
            Left.H = 0;
            Left.Owner = PlayerType.Red;
            Right.Status = Status.Untested;
            Right.G = 0;
            Right.H = 0;
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



        public bool TakeHex(Common.PlayerType owner, int row, int column)
        {
            var node = Board.FirstOrDefault(x =>
                x.Row == row && x.Column == column && x.Owner == Common.PlayerType.White);
            if (node == null)
            {
                return false;
            }

            node.Owner = owner;

            return true;
        }

        public void ReleaseHex(int row, int column)
        {
            var node = Board.FirstOrDefault(x =>
                x.Row == row && x.Column == column);
            if (node != null)
            {
                node.Owner = PlayerType.White;
            }
        }
    }
}