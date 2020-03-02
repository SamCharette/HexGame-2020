using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Players.Minimax.List
{

    public class ListNode
    {
        public int Row;
        public int Column;
        public int BoardSize;
        public Guid RandomValue;
        public List<ListNode> AdjacencyList;
        public Common.PlayerType Owner = Common.PlayerType.White;

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
            ResetAdjacencies();
        }

        public void ResetAdjacencies()
        {
            AdjacencyList = new List<ListNode>(BoardSize * BoardSize);
            AdjacencyList.Add(this);
        }

        public void UpdateAdjacencyList(List<ListNode> adjacentNodes)
        {
            AdjacencyList.Clear();
            AdjacencyList.AddRange(adjacentNodes);
        }

        public int RemainingDistance()
        {
            var bestStartNode = AdjacencyList.OrderBy(x => x.GetDistanceToStart())
                .FirstOrDefault();
            var bestEndNode = AdjacencyList.OrderBy(x => x.GetDistanceToEnd()).FirstOrDefault();

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
            var bestNode = AdjacencyList.OrderBy(x => x.RawDistanceToTop)
                .FirstOrDefault();
            return bestNode.RawDistanceToTop;
        }
        public int GetDistanceToLeft()
        {
            var bestNode = AdjacencyList.OrderBy(x => x.RawDistanceToLeft)
                .FirstOrDefault();
            return bestNode.RawDistanceToLeft;
        }
        public int GetDistanceToBottom()
        {
            // Using the adjacency graph, let's find the node connected that is closest 
            // to the top, as we are essentially that close from here because we are connected
            var bestNode = AdjacencyList.OrderBy(x => x.RawDistanceToBottom)
                .FirstOrDefault();
            return bestNode.RawDistanceToBottom;
        }

        public int GetDistanceToRight()
        {
            // Using the adjacency graph, let's find the node connected that is closest 
            // to the top, as we are essentially that close from here because we are connected
            var bestNode = AdjacencyList.OrderBy(x => x.RawDistanceToRight)
                .FirstOrDefault();
            return bestNode.RawDistanceToRight;
        }


    }
}