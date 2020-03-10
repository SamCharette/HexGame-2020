using System;
using System.Collections.Generic;
using System.Text;
using Players.Common;

namespace Players.Minimax.List
{
    public class ListHex
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public List<ListHex> Attached { get; set; }
        public int F => G + H;
        public int G { get; set; }
        public int H { get; set; }
        public Common.PlayerType Owner { get;set; }
        
        public ListHex Parent;
        public Guid RandomValue;

        public ListHex(int size, int row, int column)
        {
            Parent = null;
            RandomValue = Guid.NewGuid();
            G = 0;
            H = 0;
            
            Owner = PlayerType.White;
        }

        public void AttachTo(ListHex node)
        {
            if (!IsAttachedTo(node) && node.Owner == Owner)
            {
                Attached.Add(node);
            }
        }

        public void DetachFrom(ListHex node)
        {
            if (IsAttachedTo(node))
            {
                Attached.Remove(node);
            }
        }
        public bool IsAttachedTo(ListHex node)
        {
            return Attached.Contains(node);
        }
    }

}
