using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Players.Common;

namespace Players.Minimax.List
{

    public class ListNode
    {
        public int Row;
        public int Column;
        public int BoardSize;
        public Guid RandomValue;
        public List<ListNode> Neighbours = new List<ListNode>();
        public Common.PlayerType Owner = Common.PlayerType.White;

        public int G;
        public int H;
        public Status Status = Status.Untested;
        public ListNode Parent;

        public int F => G + H;

        public int RawDistanceToLeft => Column;
        public int RawDistanceToRight => BoardSize - 1 - Column;

        public int RawDistanceToTop => Row;

        public int RawDistanceToBottom => BoardSize - 1 - Row;



        public ListNode(int Size, int row, int column)
        {
            BoardSize = Size;
            Row = row;
            Column = column;
            RandomValue = Guid.NewGuid();
        }

        public bool IsNeighboursWith(ListNode neighbour)
        {
            return Neighbours.Any(x => x.Row == neighbour.Row && x.Column == neighbour.Column) ;
        }
        public void AttachTo(ListNode neighbour)
        {
            if (!Neighbours.Any(x => x.Row == neighbour.Row && x.Column == neighbour.Column))
            {
                Neighbours.Add(neighbour);
            }
        }
        
        public int RemainingDistance()
        {
            var bestStartNode = Neighbours.OrderBy(x => x.GetDistanceToStart())
                .FirstOrDefault();
            var bestEndNode = Neighbours.OrderBy(x => x.GetDistanceToEnd()).FirstOrDefault();

            return bestStartNode.GetDistanceToStart() + bestEndNode.GetDistanceToEnd();
        }

        public int GetDistanceToStart()
        {
            if (Owner == Common.PlayerType.Blue)
            {
                return GetDistanceToTop();
            }

            if (Owner == Common.PlayerType.Red)
            {
                return GetDistanceToLeft();
            }

            return -1;
        }

        public int GetDistanceToEnd()
        {
            if (Owner == Common.PlayerType.Blue)
            {
                return GetDistanceToBottom();
            }

            if (Owner == Common.PlayerType.Red)
            {
                return GetDistanceToRight();
            }

            return -1;
        }

        public int GetDistanceToTop()
        {
            // Using the adjacency graph, let's find the node connected that is closest 
            // to the top, as we are essentially that close from here because we are connected
            var bestNode = Neighbours.OrderBy(x => x.RawDistanceToTop)
                .FirstOrDefault();
            return bestNode.RawDistanceToTop;
        }
        public int GetDistanceToLeft()
        {
            var bestNode = Neighbours.OrderBy(x => x.RawDistanceToLeft)
                .FirstOrDefault();
            return bestNode.RawDistanceToLeft;
        }
        public int GetDistanceToBottom()
        {
            // Using the adjacency graph, let's find the node connected that is closest 
            // to the top, as we are essentially that close from here because we are connected
            var bestNode = Neighbours.OrderBy(x => x.RawDistanceToBottom)
                .FirstOrDefault();
            return bestNode.RawDistanceToBottom;
        }

        public int GetDistanceToRight()
        {
            // Using the adjacency graph, let's find the node connected that is closest 
            // to the top, as we are essentially that close from here because we are connected
            var bestNode = Neighbours.OrderBy(x => x.RawDistanceToRight)
                .FirstOrDefault();
            return bestNode.RawDistanceToRight;
        }


    }
}