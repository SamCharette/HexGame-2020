using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using Players.Common;

namespace Players.Minimax.List
{
    public class ListMap
    {
        public int Size { get; set; }
        public List<ListNode> Board { get; set; }
        public ListNode Top { get; set; }
        public ListNode Bottom { get; set; }
        public ListNode Left { get; set; }
        public ListNode Right { get; set; }


        public void Reset(int size)
        {
            Size = size;
            Board = new List<ListNode>(Size);
            Top = new ListNode(Size, -1, -1);
            Top.Owner = PlayerType.Blue;
            Bottom = new ListNode(Size, Size * 2, Size * 2 );
            Bottom.Owner = PlayerType.Blue;
            Left = new ListNode(Size, -2, -2);
            Left.Owner = PlayerType.Red;
            Right = new ListNode(Size, Size * 2, Size * 3);
            Right.Owner = PlayerType.Red;

            foreach (var node in Board)
            {

            }
        }

        public void AttachNodes(ListNode a, ListNode b)
        {
            a.AttachTo(b);
            b.AttachTo(a);
        }

        public void DetachNodes(ListNode a, ListNode b)
        {
            a.DetachFrom(b);
            b.DetachFrom(a);
        }

    }
}
