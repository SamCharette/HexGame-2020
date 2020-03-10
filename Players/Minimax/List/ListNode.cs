using System;
using System.Collections.Generic;
using System.Text;
using Players.Common;

namespace Players.Minimax.List
{
    public class ListNode
    {
        public int BoardSize { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public List<ListNode> Attached { get; set; }
        public int F => G + H;
        public int G { get; set; }
        public int H { get; set; }
        public Common.PlayerType Owner { get;set; }
        
        public ListNode Parent;
        public Guid RandomValue;

        public ListNode(int size, int row, int column)
        {
            Parent = null;
            RandomValue = Guid.NewGuid();
            G = 0;
            H = 0;
            BoardSize = size;
            Row = row;
            Column = column;
            Owner = PlayerType.White;
        }

        public void AttachTo(ListNode node)
        {
            if (!IsAttachedTo(node) && node.Owner == Owner)
            {
                Attached.Add(node);
            }
        }

        public void DetachFrom(ListNode node)
        {
            if (IsAttachedTo(node))
            {
                Attached.Remove(node);
            }
        }
        public bool IsAttachedTo(ListNode node)
        {
            return Attached.Contains(node);
        }
    }

}
